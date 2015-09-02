using System;

namespace Abp.MemoryDb.Repositories
{
    public class MemoryPrimaryKeyGenerator<TPrimaryKey>
    {
        private object _lastPk;

        public MemoryPrimaryKeyGenerator()
        {
            InitializeLastPk();
        }

        public TPrimaryKey GetNext()
        {
            lock (this)
            {
                return GetNextPrimaryKey();
            }
        }

        private void InitializeLastPk()
        {
            if (typeof(TPrimaryKey) == typeof(int))
            {
                _lastPk = 0;
            }
            else if (typeof(TPrimaryKey) == typeof(long))
            {
                _lastPk = 0L;
            }
            else if (typeof(TPrimaryKey) == typeof(short))
            {
                _lastPk = (short)0;
            }
            else if (typeof(TPrimaryKey) == typeof(byte))
            {
                _lastPk = (byte)0;
            }
            else if (typeof(TPrimaryKey) == typeof(Guid))
            {
                _lastPk = null;
            }
            else
            {
                throw new AbpException("Unsupported primary key type: " + typeof(TPrimaryKey));
            }
        }

        private TPrimaryKey GetNextPrimaryKey()
        {
            if (typeof(TPrimaryKey) == typeof(int))
            {
                _lastPk = ((int)_lastPk) + 1;
            }
            else if (typeof(TPrimaryKey) == typeof(long))
            {
                _lastPk = ((long)_lastPk) + 1L;
            }
            else if (typeof(TPrimaryKey) == typeof(short))
            {
                _lastPk = (short)(((short)_lastPk) + 1);
            }
            else if (typeof(TPrimaryKey) == typeof(byte))
            {
                _lastPk = (byte)(((byte)_lastPk) + 1);
            }
            else if (typeof(TPrimaryKey) == typeof(Guid))
            {
                _lastPk = Guid.NewGuid();
            }
            else
            {
                throw new AbpException("Unsupported primary key type: " + typeof(TPrimaryKey));
            }

            return (TPrimaryKey)_lastPk;
        }
    }
}
