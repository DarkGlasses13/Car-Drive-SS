using UnityEngine;
using UnityEngine.UI;

namespace Assets._Project.Systems.Merge
{
    public class MergeGrid : MonoBehaviour
    {
        [SerializeField] private GridLayoutGroup _grid;

        public int SlotsCount => _grid.constraintCount;
    }
}