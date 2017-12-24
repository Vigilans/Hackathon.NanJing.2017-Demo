using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HackathonBot.Models
{
    [Serializable]
    public class Procedure : IArticle
    {
        public string Title { get; set; }
        public List<Target> StagesByTargets { get; set; }
        public List<string> Notes { get; set; }
    }

    [Serializable]
    public class Target // 服务对象 未指定则用form flow提示，指定了就直接走下一步（注意服务对象还有子备注）
    {
        public string Name { get; set; }
        public string Explanation { get; set; }
        public List<Stage> Stages { get; set; }
    }

    [Serializable]
    public class Stage
    {
        public string Title { get; set; }
        public List<Content> Steps { get; set; }
    }
}
