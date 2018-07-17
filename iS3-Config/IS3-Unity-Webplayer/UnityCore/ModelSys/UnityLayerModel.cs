using System.Collections;
using System.Collections.Generic;
namespace IS3.Unity.Webplayer.UnityCore
{
    public class UnityLayer
    {
        public string rootName = "iS3Project";
        public UnityTreeModel UnityLayerModel { get; set; } = new UnityTreeModel();
        public UnityLayer()
        {
           
        }
    }
    public class UnityTreeModel
    {
        public UnityTreeModel parent { get; set; }
        public string Name { get; set; }
        public int layer { get; set; }
        public List<UnityTreeModel> childs { get; set; }
        public UnityTreeModel()
        {
            childs = new List<UnityTreeModel>();
        }
    }
    
}