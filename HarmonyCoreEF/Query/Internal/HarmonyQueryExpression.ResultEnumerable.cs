// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections;
using System.Collections.Generic;
using Harmony.Core.EF.Storage;

namespace Harmony.Core.EF.Query.Internal
{
    public partial class HarmonyQueryExpression
    {
        private sealed class ResultEnumerable : IEnumerable<DataObjectBase>
        {
            private readonly Func<DataObjectBase> _getElement;

            public ResultEnumerable(Func<DataObjectBase> getElement)
            {
                _getElement = getElement;
            }

            public IEnumerator<DataObjectBase> GetEnumerator() => new ResultEnumerator(_getElement);

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

            private sealed class ResultEnumerator : IEnumerator<DataObjectBase>
            {
                private DataObjectBase _value;
                private readonly Func<DataObjectBase> _getValue;

                public ResultEnumerator(Func<DataObjectBase> getValue)
                {
                    //TODO: this might be a weird shaper behavior
                    //_moved = _value.IsEmpty;
                    _getValue = getValue;
                }

                public bool MoveNext()
                {
                    _value = _getValue();
                    return _value != null;
                }

                public void Reset()
                {
                    throw new NotImplementedException();
                }

                object IEnumerator.Current => Current;

                public DataObjectBase Current => _value;

                void IDisposable.Dispose()
                {
                }
            }
        }
    }
}
