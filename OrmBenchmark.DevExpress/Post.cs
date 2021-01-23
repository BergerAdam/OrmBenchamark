using DevExpress.Xpo;
using OrmBenchmark.Core;
using System;

namespace OrmBenchmark.DevExpress
{

    [Persistent(mapTo: "Posts")]
    public class Post : XPLiteObject ,IPost
    {       
        public Post(Session session)
        : base(session)
        {
        }

        [Key(true)]

        public int Id
        {
            get { return fId; }
            set { SetPropertyValue(nameof(Id), ref fId, value); }
        }
        int fId;


        public string Text
        {
            get { return fText; }
            set { SetPropertyValue(nameof(Text), ref fText, value); }
        }
        string fText;


        public DateTime CreationDate
        {
            get { return fCreationDate; }
            set { SetPropertyValue(nameof(CreationDate), ref fCreationDate, value); }
        }
        DateTime fCreationDate;

        public DateTime LastChangeDate
        {
            get { return fLastChangeDate; }
            set { SetPropertyValue(nameof(LastChangeDate), ref fLastChangeDate, value); }
        }

        DateTime fLastChangeDate;


        public int? Counter1
        {
            get { return fCounter1; }
            set { SetPropertyValue(nameof(Counter1), ref fCounter1, value); }
        }

        int? fCounter1;

        public int? Counter2
        {
            get { return fCounter2; }
            set { SetPropertyValue(nameof(Counter2), ref fCounter2, value); }
        }

        int? fCounter2;

        public int? Counter3
        {
            get { return fCounter3; }
            set { SetPropertyValue(nameof(Counter3), ref fCounter3, value); }
        }

        int? fCounter3;

        public int? Counter4
        {
            get { return fCounter4; }
            set { SetPropertyValue(nameof(Counter4), ref fCounter4, value); }
        }

        int? fCounter4;

        public int? Counter5
        {
            get { return fCounter5; }
            set { SetPropertyValue(nameof(Counter5), ref fCounter5, value); }
        }

        int? fCounter5;

        public int? Counter6
        {
            get { return fCounter6; }
            set { SetPropertyValue(nameof(Counter6), ref fCounter6, value); }
        }

        int? fCounter6;

        public int? Counter7
        {
            get { return fCounter7; }
            set { SetPropertyValue(nameof(Counter7), ref fCounter7, value); }
        }

        int? fCounter7;

        public int? Counter8
        {
            get { return fCounter8; }
            set { SetPropertyValue(nameof(Counter8), ref fCounter8, value); }
        }

        int? fCounter8;

        public int? Counter9
        {
            get { return fCounter9; }
            set { SetPropertyValue(nameof(Counter9), ref fCounter9, value); }
        }

        int? fCounter9;
    }

    //
    //public class PostSqlServer : Post
    //{
    //    public PostSqlServer(Session session):base(session)
    //    {

    //    }
    //}


    //[Persistent(mapTo: "posts")]
    //public class PostMySql : Post
    //{
    //    public PostMySql(Session session) : base(session)
    //    {

    //    }
    //}

    //[Persistent(mapTo: "public.\"posts\"")]
    //public class PostPostegres : Post
    //{
    //    public PostPostegres(Session session) : base(session)
    //    {

    //    }
    //}
}
