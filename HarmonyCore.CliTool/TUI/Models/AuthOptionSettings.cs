using HarmonyCoreGenerator.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HarmonyCore.CliTool.TUI.Models
{
    public class AuthOptionSettings : IPropertyItemSetting, ISingleItemSettings
    {
        AuthOptions _wrapped;
        Action<AuthOptions> _replaceNull;
        public AuthOptionSettings(AuthOptions wrapped, Action<AuthOptions> replaceNull)
        {
            _replaceNull = replaceNull;
            _wrapped = wrapped;
            BaseInterface.LoadDisplayPropertyBacking<AuthOptionSettings>();
            RequireAuth = _wrapped?.RequireAuth;
            RequiredRoles = _wrapped?.RequiredRoles;
        }

        [IgnoreProperty]
        public string Prompt { get; set; }
        [IgnoreProperty]
        public PropertyInfo Source { get; set; }

        [IgnoreProperty]
        public object Value
        {
            get
            {
                if (RequireAuth == null && RequiredRoles == null)
                {
                    return "-";
                }
                else
                {
                    return "...";
                }
            }
            set
            {
                //ignore set
            }
        }

        [IgnoreProperty]
        public List<PropertyInfo> DisplayPropertyBacking { get; set; } = new List<PropertyInfo>();

        [IgnoreProperty]
        public SolutionInfo Context { get; set; }

        [IgnoreProperty]
        public string Name => "Auth Options";
        [Prompt("Require Auth")]
        [NullableBoolInjector]
        [NullableBoolExtractor]
        [NullableBoolOptionsExtractor]
        public bool? RequireAuth { get; set; }

        [Prompt("Required Roles")]
        public string RequiredRoles { get; set; }

        protected ISingleItemSettings BaseInterface => this;
        public void Save(SolutionInfo context)
        {
            if(_wrapped == null && (RequireAuth != null || RequiredRoles != null))
            {
                _wrapped = new AuthOptions
                {
                    RequireAuth = RequireAuth,
                    RequiredRoles = RequiredRoles,
                };
                _replaceNull(_wrapped);
            }
            else if(_wrapped != null && RequiredRoles == null && RequireAuth == null)
            {
                _replaceNull(null);
            }
            else if(_wrapped != null)
            {
                _wrapped.RequireAuth = RequireAuth;
                _wrapped.RequiredRoles = RequiredRoles;
            }
        }
    }
}
