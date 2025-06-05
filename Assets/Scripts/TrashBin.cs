using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashBin : MonoBehaviour
{
    public TipoLixo tipoAceito; // O tipo de lixo que este cesto aceita (Plastico, Metal, etc.)

    // NOVO: Adicione refer�ncias aos AudioClips para sons de dep�sito
    public AudioClip somDepositoCorreto;
    public AudioClip somDepositoIncorreto;

    // Esta fun��o � chamada quando um Collider 2D de outro objeto entra no Collider 2D deste cesto
    // O Collider 2D do cesto DEVE ter 'Is Trigger' marcado no Inspector.
    void OnTriggerEnter2D(Collider2D other)
    {
        // Tenta obter o componente PlayerController do objeto que entrou no trigger
        PlayerController player = other.GetComponent<PlayerController>();

        if (player != null) // Se o objeto que entrou no trigger for o jogador
        {
            // Pega o tipo de lixo que o jogador est� carregando
            TipoLixo lixoDoPlayer = player.GetTipoLixoCarregado();

            // Verifica se o tipo de lixo que o jogador est� carregando corresponde ao tipo aceito por este cesto
            if (lixoDoPlayer == tipoAceito)
            {
                // Se sim, o lixo pode ser depositado
                // Encontra o componente ItemLixo que � filho do jogador (o lixo que ele est� carregando)
                TrashItem lixoParaDepositar = player.transform.GetComponentInChildren<TrashItem>();
                if (lixoParaDepositar != null)
                {
                    lixoParaDepositar.Depositar(); // Chama o m�todo de dep�sito do ItemLixo
                    player.LixoDepositado(); // Notifica o PlayerController que o lixo foi solto

                    // NOVO: Toca som de dep�sito correto
                    if (AudioManager.Instance != null && somDepositoCorreto != null)
                    {
                        AudioManager.Instance.Play(somDepositoCorreto);
                    }
                }
            }
            // Se o jogador est� carregando lixo, mas � do tipo errado para este cesto
            else if (lixoDoPlayer != TipoLixo.Nenhum) // Garante que o player est� com algum lixo
            {
                Debug.Log("Lixo incorreto! Este cesto � para " + tipoAceito.ToString() + ", mas voc� tentou depositar " + lixoDoPlayer.ToString());
                // NOVO: Toca som de dep�sito incorreto
                if (AudioManager.Instance != null && somDepositoIncorreto != null)
                {
                    AudioManager.Instance.Play(somDepositoIncorreto);
                }
            }
            // Se o player n�o est� carregando lixo (lixoDoPlayer == TipoLixo.Nenhum), n�o faz nada.
        }
    }
}
