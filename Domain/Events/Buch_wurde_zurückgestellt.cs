﻿using System;
using Framework;
using Framework.Domain;

namespace Domain.Events
{
    [Serializable]
    public class Buch_wurde_zurückgestellt : Event
    {
        public Guid BuchID { get; set; }

        public override Guid AggregateId
        {
            get { return BuchID; }
        }
    }
}
