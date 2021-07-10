using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LumberCraft
{
    public class ResourceStack : MonoBehaviour
    {
        public List<GameObject> stacks;
        public int currentIndex;
        public int resourceCount;

        // Start is called before the first frame update
        void Start()
        {
            resourceCount = stacks.Count;
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void FillStack()
        {
            //ResourcesData resources = ResourcesData.SharedManager();
            //resources.AddResource((int)ResourceIndex.woodsIndex,1);

            stacks[currentIndex].SetActive(true);

            if (currentIndex >= resourceCount - 1)
            {
                return;
            }

            ++currentIndex;
            //++Constants.WoodCount;

        }

        public void ClearStack()
        {
            stacks[currentIndex].SetActive(false);

            if (currentIndex <= 0)
            {
                return;
            }

            --currentIndex;
            //--Constants.WoodCount;
        }
    }
}
