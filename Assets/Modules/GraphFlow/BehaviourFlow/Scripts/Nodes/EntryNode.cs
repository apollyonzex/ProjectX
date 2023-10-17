
using GraphNode;


namespace BehaviourFlow.Nodes {

    [System.Serializable]
    [Unique]
    public class EntryNode : BTNode {

        [Output]
        [Display("")]
        public BTChildNode root { get; set; }
       
    }
}