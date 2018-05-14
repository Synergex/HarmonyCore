// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Common;
using Microsoft.AspNet.OData.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Builder.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Internal;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.ObjectPool;
using Microsoft.OData;
using Moq;
using Moq.Protected;

namespace ODataTestHelper
{
    /// <summary>
    /// A class to create IRouteBuilder/HttpConfiguration.
    /// </summary>
    public class RoutingConfigurationFactory
    {
        /// <summary>
        /// Initializes a new instance of the routing configuration class.
        /// </summary>
        /// <returns>A new instance of the routing configuration class.</returns>
        public static IRouteBuilder Create()
        {
            IServiceCollection serviceCollection = new ServiceCollection();
            serviceCollection.AddMvcCore();
            serviceCollection.AddSingleton<ObjectPoolProvider, DefaultObjectPoolProvider>();
            serviceCollection.AddSingleton<ILoggerFactory>(NullLoggerFactory.Instance);
            serviceCollection.AddOData();
            // For routing tests, add an IActionDescriptorCollectionProvider.
            serviceCollection.AddSingleton<IActionDescriptorCollectionProvider, TestActionDescriptorCollectionProvider>();

            // Add an action select to return a default descriptor.
            var mockAction = new Mock<ActionDescriptor>();
            ActionDescriptor actionDescriptor = mockAction.Object;

            var mockActionSelector = new Mock<IActionSelector>();
            mockActionSelector
                .Setup(a => a.SelectCandidates(It.IsAny<RouteContext>()))
                .Returns(new ActionDescriptor[] { actionDescriptor });

            mockActionSelector
                .Setup(a => a.SelectBestCandidate(It.IsAny<RouteContext>(), It.IsAny<IReadOnlyList<ActionDescriptor>>()))
                .Returns(actionDescriptor);

            // Add a mock action invoker & factory.
            var mockInvoker = new Mock<IActionInvoker>();
            mockInvoker.Setup(i => i.InvokeAsync())
                .Returns(Task.FromResult(true));

            var mockInvokerFactory = new Mock<IActionInvokerFactory>();
            mockInvokerFactory.Setup(f => f.CreateInvoker(It.IsAny<ActionContext>()))
                .Returns(mockInvoker.Object);

            // Create a logger, diagnostic source and app builder.
            var mockLoggerFactory = new Mock<ILoggerFactory>();
            var diagnosticSource = new DiagnosticListener("Microsoft.AspNetCore");
            IApplicationBuilder appBuilder = new ApplicationBuilder(serviceCollection.BuildServiceProvider());

            // Create a route build with a default path handler.
            IRouteBuilder routeBuilder = new RouteBuilder(appBuilder);
            routeBuilder.DefaultHandler = new MvcRouteHandler(
                mockInvokerFactory.Object,
                mockActionSelector.Object,
                diagnosticSource,
                mockLoggerFactory.Object,
                new ActionContextAccessor());

            routeBuilder.EnableDependencyInjection();
            routeBuilder.Filter();
            routeBuilder.Expand();
            routeBuilder.Select();
            return routeBuilder;
        }

        /// <summary>
        /// Initializes a new instance of the routing configuration class.
        /// </summary>
        /// <returns>A new instance of the routing configuration class.</returns>
        public static IRouteBuilder CreateWithRoute(string route)
        {
            // TODO: Need to add the route to the prefix.
            IRouteBuilder routeBuilder = Create();
            return routeBuilder;
        }

        /// <summary>
        /// Initializes a new instance of the routing configuration class.
        /// </summary>
        /// <returns>A new instance of the routing configuration class.</returns>
        public static IRouteBuilder CreateWithRootContainer(string routeName, Action<IContainerBuilder> configureAction = null)
        {
            IRouteBuilder builder = Create();
            if (!string.IsNullOrEmpty(routeName))
            {
                // Build and configure the root container.
                IPerRouteContainer perRouteContainer = builder.ServiceProvider.GetRequiredService<IPerRouteContainer>();
                if (perRouteContainer == null)
                {
                    throw new ArgumentNullException();
                }

                // Create an service provider for this route. Add the default services to the custom configuration actions.
                Action<IContainerBuilder> builderAction = typeof(ODataRouteBuilderExtensions).GetMethod("ConfigureDefaultServices", BindingFlags.NonPublic | BindingFlags.Static).Invoke(null, new object[] { builder, configureAction }) as Action<IContainerBuilder>;
                IServiceProvider serviceProvider = perRouteContainer.CreateODataRootContainer(routeName, builderAction);
            }

            return builder;
        }

        /// <summary>
        /// Initializes a new instance of the routing configuration class.
        /// </summary>
        /// <returns>A new instance of the routing configuration class.</returns>
        public static IRouteBuilder CreateWithTypes(params Type[] types)
        {
            IRouteBuilder builder = Create();
            builder.Count().OrderBy().Filter().Expand().MaxTop(null);

            ApplicationPartManager applicationPartManager = builder.ApplicationBuilder.ApplicationServices.GetRequiredService<ApplicationPartManager>();
            AssemblyPart part = new AssemblyPart(new MockAssembly(types));
            applicationPartManager.ApplicationParts.Add(part);

            return builder;
        }

        /// <summary>
        /// Initializes a new instance of the routing configuration class.
        /// </summary>
        /// <returns>A new instance of the routing configuration class.</returns>
        public static IRouteBuilder CreateWithRootContainerAndTypes(string routeName = null, Action<IContainerBuilder> configureAction = null, params Type[] types)
        {
            IRouteBuilder builder = CreateWithRootContainer(routeName, configureAction);

            ApplicationPartManager applicationPartManager = builder.ApplicationBuilder.ApplicationServices.GetRequiredService<ApplicationPartManager>();
            AssemblyPart part = new AssemblyPart(new MockAssembly(types));
            applicationPartManager.ApplicationParts.Add(part);

            return builder;
        }
    }

    /// <summary>
    /// Test version of an <see cref="IActionDescriptorCollectionProvider"/>
    /// </summary>
    internal class TestActionDescriptorCollectionProvider : IActionDescriptorCollectionProvider
    {
        /// <summary>
        /// A list of <see cref="ActionDescriptor"/> that we can modify.
        /// </summary>
        public List<ActionDescriptor> TestActionDescriptors { get; } = new List<ActionDescriptor>();

        /// <summary>
        /// The action descriptors collection from <see cref="IActionDescriptorCollectionProvider"/>
        /// which is immutable.
        /// </summary>
        public ActionDescriptorCollection ActionDescriptors
        {
            get
            {
                return new ActionDescriptorCollection(TestActionDescriptors, 0);
            }
        }
    }

    public sealed class MockType : Mock<Type>
    {
        public static implicit operator Type(MockType mockType)
        {
            return mockType.Object;
        }

        private readonly List<MockPropertyInfo> _propertyInfos = new List<MockPropertyInfo>();
        private MockType _baseType;

        public MockType()
            : this("T")
        {
        }

        public MockType(string typeName, bool hasDefaultCtor = true, string @namespace = "DefaultNamespace")
        {
            SetupGet(t => t.Name).Returns(typeName);
            SetupGet(t => t.BaseType).Returns(typeof(Object));
            SetupGet(t => t.Assembly).Returns(typeof(object).Assembly);
            Setup(t => t.GetProperties(It.IsAny<BindingFlags>()))
                .Returns(() => _propertyInfos.Union(_baseType != null ? _baseType._propertyInfos : Enumerable.Empty<MockPropertyInfo>()).Select(p => p.Object).ToArray());
            Setup(t => t.Equals(It.IsAny<object>())).Returns<Type>(t => ReferenceEquals(Object, t));
            Setup(t => t.ToString()).Returns(typeName);
            Setup(t => t.Namespace).Returns(@namespace);
            Setup(t => t.IsAssignableFrom(It.IsAny<Type>())).Returns(true);
            Setup(t => t.FullName).Returns(@namespace + "." + typeName);

            TypeAttributes(System.Reflection.TypeAttributes.Class | System.Reflection.TypeAttributes.Public);


            if (hasDefaultCtor)
            {
                this.Protected()
                    .Setup<ConstructorInfo>(
                        "GetConstructorImpl",
                        BindingFlags.Instance | BindingFlags.Public,
                        ItExpr.IsNull<Binder>(),
                        CallingConventions.Standard | CallingConventions.VarArgs,
                        Type.EmptyTypes,
                        ItExpr.IsNull<ParameterModifier[]>())
                    .Returns(new Mock<ConstructorInfo>().Object);
            }
        }

        public MockType TypeAttributes(TypeAttributes typeAttributes)
        {
            this.Protected()
                .Setup<TypeAttributes>("GetAttributeFlagsImpl")
                .Returns(typeAttributes);

            return this;
        }

        public MockType BaseType(MockType mockBaseType)
        {
            _baseType = mockBaseType;
            SetupGet(t => t.BaseType).Returns(mockBaseType);
            Setup(t => t.IsSubclassOf(mockBaseType)).Returns(true);

            return this;
        }

        public MockType Property<T>(string propertyName)
        {
            Property(typeof(T), propertyName);

            return this;
        }

        public MockType Property(Type propertyType, string propertyName, params Attribute[] attributes)
        {
            var mockPropertyInfo = new MockPropertyInfo(propertyType, propertyName);
            mockPropertyInfo.SetupGet(p => p.DeclaringType).Returns(this);
            mockPropertyInfo.SetupGet(p => p.ReflectedType).Returns(this);
            mockPropertyInfo.Setup(p => p.GetCustomAttributes(It.IsAny<bool>())).Returns(attributes);

            _propertyInfos.Add(mockPropertyInfo);

            return this;
        }

        public MockPropertyInfo GetProperty(string name)
        {
            return _propertyInfos.Single(p => p.Object.Name == name);
        }

        public MockType AsCollection()
        {
            var mockCollectionType = new MockType();

            mockCollectionType.Setup(t => t.GetInterfaces()).Returns(new Type[] { typeof(IEnumerable<>).MakeGenericType(this) });

            return mockCollectionType;
        }
    }

    public sealed class MockPropertyInfo : Mock<PropertyInfo>
    {
        private readonly Mock<MethodInfo> _mockGetMethod = new Mock<MethodInfo>();
        private readonly Mock<MethodInfo> _mockSetMethod = new Mock<MethodInfo>();

        public static implicit operator PropertyInfo(MockPropertyInfo mockPropertyInfo)
        {
            return mockPropertyInfo.Object;
        }

        public MockPropertyInfo()
            : this(typeof(object), "P")
        {
        }

        public MockPropertyInfo(Type propertyType, string propertyName)
        {
            SetupGet(p => p.DeclaringType).Returns(typeof(object));
            SetupGet(p => p.ReflectedType).Returns(typeof(object));
            SetupGet(p => p.Name).Returns(propertyName);
            SetupGet(p => p.PropertyType).Returns(propertyType);
            SetupGet(p => p.CanRead).Returns(true);
            SetupGet(p => p.CanWrite).Returns(true);
            Setup(p => p.GetGetMethod(It.IsAny<bool>())).Returns(_mockGetMethod.Object);
            Setup(p => p.GetSetMethod(It.IsAny<bool>())).Returns(_mockSetMethod.Object);
            Setup(p => p.Equals(It.IsAny<object>())).Returns<PropertyInfo>(p => ReferenceEquals(Object, p));

            _mockGetMethod.SetupGet(m => m.Attributes).Returns(MethodAttributes.Public);
        }

        public MockPropertyInfo Abstract()
        {
            _mockGetMethod.SetupGet(m => m.Attributes)
                .Returns(_mockGetMethod.Object.Attributes | MethodAttributes.Abstract);

            return this;
        }
    }

    public sealed class MockAssembly : Assembly
    {
        Type[] _types;

        public MockAssembly(params Type[] types)
        {
            _types = types;
        }

        public MockAssembly(params MockType[] types)
        {
            foreach (var type in types)
            {
                type.SetupGet(t => t.Assembly).Returns(this);
            }
            _types = types.Select(t => t.Object).ToArray();
        }

        /// <remarks>
        /// AspNet uses GetTypes as opposed to DefinedTypes()
        /// </remarks>
        public override Type[] GetTypes()
        {
            return _types;
        }

        /// <remarks>
        /// AspNetCore uses DefinedTypes as opposed to GetTypes()
        /// </remarks>
        public override IEnumerable<TypeInfo> DefinedTypes
        {
            get { return _types.AsEnumerable().Select(a => a.GetTypeInfo()); }
        }
    }
}