using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashItem : MonoBehaviour
{
    public TipoLixo tipoLixo;
    private bool coletado = false;

    public void Coletar(Transform paiDoJogador)
    {
        if (!coletado)
        {
            coletado = true;
            transform.SetParent(paiDoJogador);
            transform.localPosition = Vector3.zero;

            if (GetComponent<Collider2D>() != null)
            {
                GetComponent<Collider2D>().enabled = false;
            }
        }
    }

    public void Depositar()
    {
        transform.SetParent(null);
        this.gameObject.SetActive(false);

        // AQUI ESTÁ A MUDANÇA: Agora chama GameManager.Instance
        if (GameManager.Instance != null)
        {
            GameManager.Instance.LixoColetado(); // O método LixoColetado ainda existe no GameManager
        }
    }
}
