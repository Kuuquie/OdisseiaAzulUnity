using System.Collections;
using System.Collections.Generic;
using UnityEngine;

 public class TrashItem : MonoBehaviour
 {
    public TipoLixo tipoLixo; // Define o tipo deste item de lixo (Plastico, Metal, etc.)
    private bool coletado = false; // Flag para saber se este lixo já foi pego pelo jogador

    public AudioClip somDeColeta; // Som reproduzido ao coletar este lixo

    // Método para ser chamado pelo PlayerController quando o lixo é coletado
    public void Coletar(Transform paiDoJogador)
    {
        if (!coletado) // Garante que o lixo só seja coletado uma vez
        {
            coletado = true;
            // Torna o lixo filho do GameObject do jogador para que ele o siga
            transform.SetParent(paiDoJogador);
            // Ajusta a posição local para o lixo aparecer "no" jogador
            transform.localPosition = Vector3.zero;

            // Desabilita o Collider 2D para que o lixo não possa ser clicado novamente enquanto está sendo carregado
            if (GetComponent<Collider2D>() != null)
            {
                GetComponent<Collider2D>().enabled = false;
            }
            // Opcional: Desabilitar o SpriteRenderer ou mudar para uma versão "coletada" do sprite

            // Toca o som de coleta usando o AudioManager
            if (AudioManager.Instance != null && somDeColeta != null)
            {
                AudioManager.Instance.Play(somDeColeta);
            }
        }
    }

    // Método para ser chamado pelo CestoLixo quando o lixo é depositado corretamente
    public void Depositar()
    {
        // Remove o lixo do pai (jogador), tornando-o novamente um objeto de nível superior
        transform.SetParent(null);
        this.gameObject.SetActive(false); // Faz o lixo "desaparecer" da cena

        // Notifica o GameManager que um lixo foi depositado
        if (GameManager.Instance != null)
        {
            GameManager.Instance.LixoColetado();
        }
    }
}
