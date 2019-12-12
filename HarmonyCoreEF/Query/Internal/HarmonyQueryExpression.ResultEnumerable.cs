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

            public IEnumerator<DataObjectBase> GetEnumerator() => new ResultEnumerator(_getElement());

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

            private sealed class ResultEnumerator : IEnumerator<DataObjectBase>
            {
                private readonly DataObjectBase _value;
                private bool _moved;

                public ResultEnumerator(DataObjectBase value)
                {
                    _value = value;
                    _moved = _value == null;
                }

                public bool MoveNext()
                {
                    if (!_moved)
                    {
                        _moved = true;

                        return _moved;
                    }

                    return false;
                }

                public void Reset()
                {
                    _moved = false;
                }

                object IEnumerator.Current => Current;

                public DataObjectBase Current => !_moved ? null : _value;

                void IDisposable.Dispose()
                {
                }
            }
        }
    }
}
