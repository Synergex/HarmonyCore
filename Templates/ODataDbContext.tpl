<CODEGEN_FILENAME>DbContext.dbl</CODEGEN_FILENAME>
<REQUIRES_USERTOKEN>MODELS_NAMESPACE</REQUIRES_USERTOKEN>
;//****************************************************************************
;//
;// Title:       ODataDbContext.tpl
;//
;// Type:        CodeGen Template
;//
;// Description: Used to create OData DbContext classes in a Harmony Core environment
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
;; Title:       DbContext.dbl
;;
;; Type:        Class
;;
;; Description: OData DbContext class
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

import Harmony.Core
import Harmony.Core.Context
import Microsoft.EntityFrameworkCore
import System.Linq.Expressions
import <MODELS_NAMESPACE>

namespace <NAMESPACE>

	;;; <summary>
	;;; 
	;;; </summary>
	public class DbContext extends Microsoft.EntityFrameworkCore.DbContext
	
		mDataProvider, @IDataObjectProvider

		;;; <summary>
		;;; Construct a new DbContext.
		;;; </summary>
		public method DbContext
			options, @DbContextOptions<DbContext>
			dataProvider, @IDataObjectProvider
			endparams
			parent(options)
		proc
			mDataProvider = dataProvider
		endmethod

		<STRUCTURE_LOOP>
		;;; <summary>
		;;; Exposes <StructureNoplural> data.
		;;; </summary>
		public readwrite property <StructurePlural>, @DbSet<<StructureNoplural>>

		</STRUCTURE_LOOP>
		;;; <summary>
		;;; 
		;;; </summary>
		protected override method OnConfiguring, void
			opts, @DbContextOptionsBuilder
		proc
			HarmonyDbContextOptionsExtensions.UseHarmonyDatabase(opts, mDataProvider)
		endmethod

		;;; <summary>
		;;; 
		;;; </summary>
		protected override method OnModelCreating, void
			parm, @ModelBuilder
		proc
			parm.Ignore(^typeof(AlphaDesc))
			parm.Ignore(^typeof(DataObjectMetadataBase))

.region "Tag filtering"

			;;This will currently only work for single field==value tags.

			<STRUCTURE_LOOP>
			<IF STRUCTURE_TAGS>
			<TAG_LOOP>
			data <structureNoplural>Param = Expression.Parameter(^typeof(<structureNoplural>))
			parm.Entity(^typeof(<structureNoplural>)).HasQueryFilter(Expression.Lambda(Expression.Block(Expression.Equal(Expression.Call(^typeof(EF), "Property", new Type[#] { ^typeof(<TAGLOOP_FIELD_CSTYPE>) }, <structureNoplural>Param, Expression.Constant("<TagloopFieldName>")), Expression.Constant(<TAGLOOP_TAG_VALUE>))), new ParameterExpression[#] { <structureNoplural>Param }))

			</TAG_LOOP>
			</IF STRUCTURE_TAGS>
			</STRUCTURE_LOOP>
.endregion

.region "Relationships to other structures"

			<STRUCTURE_LOOP>
			<IF STRUCTURE_RELATIONS>
			<RELATION_LOOP>
			<IF TWO_WAY_ONE_TO_ONE>
	        ;; One to one relationship from structure <StructureNoplural> key <RELATION_FROMKEY> to structure <RelationTostructure>.<RELATION_TOKEY> with a backward relationship
			parm.Entity(^typeof(<StructureNoplural>)).HasOne(^typeof(<RelationTostructureNoplural>), "<StructurePlural>").WithOne("<RelationFromkey>Data")
			</IF TWO_WAY_ONE_TO_ONE>
			<IF ONE_WAY_ONE_TO_ONE>
	        ;; One to one relationship from structure <StructureNoplural> key <RELATION_FROMKEY> to structure <RelationTostructure>.<RELATION_TOKEY> without a backward relationship
			;TODO: Code needed here!!!
			</IF ONE_WAY_ONE_TO_ONE>
			<IF TWO_WAY_ONE_TO_MANY>
	        ;; One to many relationship from structure <StructureNoplural> key <RELATION_FROMKEY> to structure <RelationTostructure> key <RELATION_TOKEY> with a backward relationship
			;TODO: Code needed here!!!
			</IF TWO_WAY_ONE_TO_MANY>
			<IF ONE_WAY_ONE_TO_MANY>
	        ;; One to many relationship from structure <StructureNoplural> key <RELATION_FROMKEY> to structure <RelationTostructure> key <RELATION_TOKEY> without a backward relationship
			;TODO: Code needed here!!!
			</IF ONE_WAY_ONE_TO_MANY>

			</RELATION_LOOP>
			</IF STRUCTURE_RELATIONS>
			</STRUCTURE_LOOP>
.endregion
			parent.OnModelCreating(parm)

		endmethod

	endclass

endnamespace