﻿using OrmBenchmark.Core;
using ServiceStack.DataAnnotations;
using System;

namespace OrmBenchmark.OrmLite
{
    [Alias("Posts")]
    public class Post : IPost
    {
        public int Id { get; set; }
        public string Text { get; set; }

        [Alias("CreationDate")]
        public DateTime CreationDate { get; set; }

        [Alias("CreationDate")]
        public DateTime LastChangeDate { get; set; }
        public int? Counter1 { get; set; }
        public int? Counter2 { get; set; }
        public int? Counter3 { get; set; }
        public int? Counter4 { get; set; }
        public int? Counter5 { get; set; }
        public int? Counter6 { get; set; }
        public int? Counter7 { get; set; }
        public int? Counter8 { get; set; }
        public int? Counter9 { get; set; }
        //public int NotExistColumn { get; set; }
    }
}
