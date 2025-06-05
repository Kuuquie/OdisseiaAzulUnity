using System.Collections;
using System.Collections.Generic;
using UnityEngine;

 public class TrashItem : MonoBehaviour
{
    public TipoLixo tipoLixo; // Define o tipo deste item de lixo (Plastico, Metal, etc.)
    private bool coletado = false; // Flag para saber se este lixo já foi pego pelo jogador

    // NOVO: Adicione uma referência ao AudioClip para o som de coleta
    public AudioClip somDeColeta;

    // Método para ser chamado pelo PlayerController quando o lixo é coletado
    public void Coletar(Transform paiDoJogador)
    {
        if (!coletado) // Garante que o lixo só seja coletado uma vez
        {
            coletado = true;
            // Torna o lixo filho do GameObject do jogador para que ele o siga
            transform.SetParent(paiDoJogador);
            // Ajusta a posição local para o lixo aparecer "no" jogador
            transform.localPosition = Vector3.zero; // Pode ajustar para new Vector3(0.5f, 0.5f, 0) para um pequeno offset

            // Desabilita o Collider 2D para que o lixo não possa ser clicado novamente enquanto está sendo carregado
            if (GetComponent<Collider2D>() != null)
            {
                GetComponent<Collider2D>().enabled = false;
            }
            // Opcional: Desabilitar o SpriteRenderer ou mudar para uma versão "coletada" do sprite

            // NOVO: Toca o som de coleta
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
