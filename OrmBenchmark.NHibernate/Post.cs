using FluentNHibernate.Mapping;
using OrmBenchmark.Core;
using System;

namespace OrmBenchmark.NHibernate
{
    public class Post : IPost
    {
        public virtual int Id { get; set; }
        public virtual string Text { get; set; }
        public virtual DateTime CreationDate { get; set; }
        public virtual DateTime LastChangeDate { get; set; }
        public virtual int? Counter1 { get; set; }
        public virtual int? Counter2 { get; set; }
        public virtual int? Counter3 { get; set; }
        public virtual int? Counter4 { get; set; }
        public virtual int? Counter5 { get; set; }
        public virtual int? Counter6 { get; set; }
        public virtual int? Counter7 { get; set; }
        public virtual int? Counter8 { get; set; }
        public virtual int? Counter9 { get; set; }
        //public virtual  int NotExistColumn { get; set; }
    }

    public class PostMap : ClassMap<Post>
    {
        public PostMap()
        {
            Table("Posts");
            Id(x => x.Id);
            Map(x => x.Text).Not.Nullable();
            Map(x => x.CreationDate).Not.Nullable();
            Map(x => x.LastChangeDate).Not.Nullable();
            Map(x => x.Counter1).Nullable();
            Map(x => x.Counter2).Nullable();
            Map(x => x.Counter3).Nullable();
            Map(x => x.Counter4).Nullable();
            Map(x => x.Counter5).Nullable();
            Map(x => x.Counter6).Nullable();
            Map(x => x.Counter7).Nullable();
            Map(x => x.Counter8).Nullable();
            Map(x => x.Counter9).Nullable();
        }
    }
}