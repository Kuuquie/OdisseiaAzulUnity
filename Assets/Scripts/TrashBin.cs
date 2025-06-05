using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashBin : MonoBehaviour
{
    public TipoLixo tipoAceito; // O tipo de lixo que este cesto aceita (Plastico, Metal, etc.)

    // Esta função é chamada quando um Collider 2D de outro objeto entra no Collider 2D deste cesto
    // O Collider 2D do cesto DEVE ter 'Is Trigger' marcado no Inspector.
    void OnTriggerEnter2D(Collider2D other)
    {
        // Tenta obter o componente PlayerController do objeto que entrou no trigger
        PlayerController player = other.GetComponent<PlayerController>();

        if (player != null) // Se o objeto que entrou no trigger for o jogador
        {
            // Pega o tipo de lixo que o jogador está carregando
            TipoLixo lixoDoPlayer = player.GetTipoLixoCarregado();

            // Verifica se o tipo de lixo que o jogador está carregando corresponde ao tipo aceito por este cesto
            if (lixoDoPlayer == tipoAceito)
            {
                // Se sim, o lixo pode ser depositado
                // Encontra o componente ItemLixo que é filho do jogador (o lixo que ele está carregando)
                TrashItem lixoParaDepositar = player.transform.GetComponentInChildren<TrashItem>();
                if (lixoParaDepositar != null)
                {
                    lixoParaDepositar.Depositar(); // Chama o método de depósito do ItemLixo
                    player.LixoDepositado(); // Notifica o PlayerController que o lixo foi solto
                }
            }
            // Se o jogador está carregando lixo, mas é do tipo errado para este cesto
            else if (lixoDoPlayer != TipoLixo.Nenhum)
            {
                Debug.Log("Lixo incorreto! Este cesto é para " + tipoAceito.ToString() + ", mas você tentou depositar " + lixoDoPlayer.ToString());
                // Opcional: Você pode adicionar aqui um efeito visual, som de erro, ou uma mensagem para o jogador
            }
        }
    }
}
