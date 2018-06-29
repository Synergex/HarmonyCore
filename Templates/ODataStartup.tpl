<CODEGEN_FILENAME>Startup.dbl</CODEGEN_FILENAME>
<REQUIRES_USERTOKEN>MODELS_NAMESPACE</REQUIRES_USERTOKEN>
<REQUIRES_CODEGEN_VERSION>5.3.3</REQUIRES_CODEGEN_VERSION>
;//****************************************************************************
;//
;// Title:       ODataEdmBuilder.tpl
;//
;// Type:        CodeGen Template
;//
;// Description: Creates a Startup class for an OData / Web API hosting environment
;//
;// Copyright (c) 2018, Synergex International, Inc. All rights reserved.
;//
;// Redistribution and use in source and binary forms, with or without
;// modification, are permitted provided that the following conditions are met:
;//
;// * Redistributions of source code must retain the above copyright notice,
;//   this list of conditions and the following disclaimer.
;//
;// * Redistributions in binary form must reproduce the above copyright notice,
;//   this list of conditions and the following disclaimer in the documentation
;//   and/or other materials provided with the distribution.
;//
;// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
;// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
;// IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
;// ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE
;// LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR
;// CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF
;// SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS
;// INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN
;// CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
;// ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE
;// POSSIBILITY OF SUCH DAMAGE.
;//
;;*****************************************************************************
;;
;; Title:       Startup.dbl
;;
;; Type:        Class
;;
;; Description: Startup class for an OData / Web API hosting environment
;;
;;*****************************************************************************
;; WARNING
;;
;; This file was code generated. Avoid editing this file, as any changes that
;; you make will be lost of the file is re-generated.
;;
;;*****************************************************************************
;;
;; Copyright (c) 2018, Synergex International, Inc.
;; All rights reserved.
;;
;; Redistribution and use in source and binary forms, with or without
;; modification, are permitted provided that the following conditions are met:
;;
;; * Redistributions of source code must retain the above copyright notice,
;;   this list of conditions and the following disclaimer.
;;
;; * Redistributions in binary form must reproduce the above copyright notice,
;;   this list of conditions and the following disclaimer in the documentation
;;   and/or other materials provided with the distribution.
;;
;; THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
;; AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
;; IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
;; ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE
;; LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR
;; CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF
;; SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS
;; INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN
;; CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
;; ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE
;; POSSIBILITY OF SUCH DAMAGE.
;;
;;*****************************************************************************
;; 
;;  This environment requires the following NuGet packages:
;;
;;  Microsoft.AspNetCore.Mvc.Core
;;  Microsoft.AspNetCore.OData
;;  Microsoft.EntityFrameworkCore
;;  Microsoft.EntityFrameworkCore.Relational
;;  Microsoft.OData.Core
;;  Microsoft.OData.Edm
;;  Microsoft.Spatial
;;  system.text.encoding.codepages
;;  Swashbuckle.AspNetCore
;;  Swashbuckle.OData
;;

import Harmony.Core.Context
import Harmony.Core.FileIO
import Harmony.Core.Utility
import Harmony.AspNetCore.Context
import Microsoft.Extensions.DependencyInjection
import Microsoft.EntityFrameworkCore
import Microsoft.AspNet.OData.Extensions
import Microsoft.AspNetCore.Builder
import Microsoft.AspNetCore.Hosting
import Swashbuckle.AspNetCore.Swagger
import <MODELS_NAMESPACE>

namespace <NAMESPACE>

	public class Startup

		public method ConfigureServices, void
			services, @IServiceCollection 
		proc

			;;Load Harmony Core

			lambda AddDataObjectMappings(serviceProvider)
			begin
				data objectProvider = new DataObjectProvider(serviceProvider.GetService<IFileChannelManager>())
				<STRUCTURE_LOOP>
				objectProvider.AddDataObjectMapping<<StructureNoplural>>("<FILE_NAME>", <IF STRUCTURE_ISAM>FileOpenMode.UpdateIndexed</IF STRUCTURE_ISAM><IF STRUCTURE_RELATIVE>FileOpenMode.UpdateRelative</IF STRUCTURE_RELATIVE>)
				</STRUCTURE_LOOP>
				mreturn objectProvider
			end

			services.AddSingleton<IFileChannelManager, FileChannelManager>()
			services.AddSingleton<IDataObjectProvider>(AddDataObjectMappings)
			services.AddSingleton<DbContextOptions<DBContext>>(new DbContextOptions<DBContext>())
			services.AddSingleton<DBContext, DBContext>()

			;;Load OData and ASP.NET

			services.AddOData()
			services.AddMvcCore()

;
;			;;Can't currently make this work with ODataController
;
;			;;Load Swagger API documentation services
;
;			services.AddMvcCore().AddApiExplorer()
;
;			lambda configureSwaggerGen(config)
;			begin
;				config.SwaggerDoc("v1", new Info() { Title = "My API", Version = "v1" })
;			end
;
;			services.AddSwaggerGen(configureSwaggerGen)

		endmethod

		public method Configure, void
			app, @IApplicationBuilder
		proc

;			;;Add the middleware to generate API documentation to a file
;
;			app.UseSwagger()

			app.UseLogging(DebugLogSession.Logging)

			;;Configure the MVC / OData environment

			data model = EdmBuilder.GetEdmModel()

			lambda MVCBuilder(builder)
			begin
				builder.Select().Expand().Filter().OrderBy().MaxTop(100).Count()
				builder.MapODataServiceRoute("odata", "odata", model)
			end

			;;Add the MVC middleware
			app.UseMvc(MVCBuilder)

;			;;Add middleware to generate Swagger UI for documentation ("available at /swagger")
;			lambda configureSwaggerUi(config)
;			begin
;				config.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1")
;			end
;			app.UseSwaggerUI(configureSwaggerUi)

		endmethod
	endclass

endnamespace
