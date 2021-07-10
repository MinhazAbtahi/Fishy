using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace LumberCraft
{
    public class ProductLand : MonoBehaviour
    {
        public List<Transform> products;
        public GameObject productPrefab;
        public ParticleSystem loadProductEffect;
        private GameObject animProduct;

        int pathIndex = 0;
        string[] animParam = { "Idle", "Walk", "Attack" };

        public void LoadProduct()
        {
            if (pathIndex > products.Count - 1) return;
            animProduct = Instantiate(productPrefab, products[pathIndex].transform);
            pathIndex++;
            loadProductEffect.Play();
            StartCoroutine(ProductAnimSeq());

            animProduct.transform.localScale = Vector3.one * 1.25f;
        }


        private IEnumerator ProductAnimSeq()
        {
            Quaternion quaternion = animProduct.transform.rotation;
            Animator animator = animProduct.GetComponent<Animator>();

            animator.SetTrigger(animParam[0]);

            float currentPosY = animator.gameObject.transform.position.y;
            float upPosY = currentPosY + 15;

            while (this)
            {
                yield return new WaitForSeconds(Random.Range(3, 5));

                Transform animTransform = animator.gameObject.transform;

                Sequence sequence = DOTween.Sequence();
                animator.SetBool(animParam[1], true);
                var check = new System.Random().Next() % 2 == 0;
                var yRotValue = check ? 360 : -360;
                sequence.Append(animTransform.DOLocalMoveY(upPosY, 2.5f)).AppendInterval(1.0f).
                    Append(animTransform.DOLocalRotate(new Vector3(quaternion.x + 0, quaternion.y + yRotValue, quaternion.z + 0), 2.0f, RotateMode.LocalAxisAdd)).
                    AppendInterval(1.0f).
                    Append(animTransform.DOLocalMoveY(currentPosY, 2.5f)).
                    OnComplete(() =>
                    {
                        animator.SetBool(animParam[1], false);
                    });


                yield return new WaitForSeconds(Random.Range(10, 12));
            }
        }
    }
}
