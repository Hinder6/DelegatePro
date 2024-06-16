using System;
using SQLite.Net.Attributes;

namespace DelegatePro.PCL
{
    public abstract class BaseDataItem
    {
        public abstract Guid ID { get; set; }
    }

    public abstract class DataItem : BaseDataItem
	{
        [PrimaryKey]
        public override Guid ID { get; set; }
	}

    public abstract class DataItemInt
    {
        [PrimaryKey]
        public abstract int ID { get; set; }
    }
}

