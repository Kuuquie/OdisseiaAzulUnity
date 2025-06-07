using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashBin : MonoBehaviour
{
    public TipoLixo tipoAceito;

    public AudioClip somDepositoCorreto;
    public AudioClip somDepositoIncorreto;


    void OnTriggerEnter2D(Collider2D other)
    {
        
        PlayerController player = other.GetComponent<PlayerController>();

        if (player != null) 
        {
            
            TipoLixo lixoDoPlayer = player.GetTipoLixoCarregado();

            
            if (lixoDoPlayer == tipoAceito)
            {
            
                TrashItem lixoParaDepositar = player.transform.GetComponentInChildren<TrashItem>();
                if (lixoParaDepositar != null)
                {
                    lixoParaDepositar.Depositar();
                    player.LixoDepositado();

                  
                    if (AudioManager.Instance != null && somDepositoCorreto != null)
                    {
                        AudioManager.Instance.Play(somDepositoCorreto);
                    }
                }
            }
            
            else if (lixoDoPlayer != TipoLixo.Nenhum)
            {
                Debug.Log("Lixo incorreto! Este cesto é para " + tipoAceito.ToString() + ", mas você tentou depositar " + lixoDoPlayer.ToString());
                // Toca o som de depósito incorreto
                if (AudioManager.Instance != null && somDepositoIncorreto != null)
                {
                    AudioManager.Instance.Play(somDepositoIncorreto);
                }
            }
            
        }
    }
}
