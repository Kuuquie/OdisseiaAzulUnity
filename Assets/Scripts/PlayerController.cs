using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float velocidadeMovimento = 5.0f;
    private Vector3 targetPosition;
    private TrashItem lixoCarregado = null;
    private bool estaMovendoParaColetar = false;
    private bool estaMovendoParaDepositar = false;

    // NOVO: Adiciona uma refer�ncia ao lixo que o peixe *est� indo buscar*
    private TrashItem lixoAlvoParaColetar = null;

    void Start()
    {
        targetPosition = transform.position;
    }

    void Update()
    {
        // Detecta o clique do mouse
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 clickPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            clickPosition.z = transform.position.z;

            RaycastHit2D hit = Physics2D.Raycast(clickPosition, Vector2.zero);

            // Reseta flags de movimento para garantir que novas a��es n�o fiquem presas em estados antigos
            estaMovendoParaColetar = false;
            estaMovendoParaDepositar = false;
            lixoAlvoParaColetar = null; // Limpa o alvo de coleta no in�cio de um novo clique

            if (lixoCarregado == null) // O jogador N�O est� carregando lixo
            {
                if (hit.collider != null)
                {
                    TrashItem lixoClicado = hit.collider.GetComponent<TrashItem>();
                    TrashBin cestoClicado = hit.collider.GetComponent<TrashBin>(); // Pode ser clicado por engano

                    if (lixoClicado != null) // Clicou em um lixo
                    {
                        targetPosition = lixoClicado.transform.position;
                        lixoAlvoParaColetar = lixoClicado; // Define o lixo alvo
                        estaMovendoParaColetar = true;
                    }
                    else // Clicou em algo que n�o � lixo (ou � um cesto vazio)
                    {
                        targetPosition = clickPosition; // Move livremente
                    }
                }
                else // Clicou em um lugar vazio
                {
                    targetPosition = clickPosition; // Move livremente
                }
            }
            else // O jogador EST� carregando lixo
            {
                if (hit.collider != null)
                {
                    TrashBin cestoClicado = hit.collider.GetComponent<TrashBin>();

                    if (cestoClicado != null) // Clicou em um cesto
                    {
                        targetPosition = cestoClicado.transform.position;
                        estaMovendoParaDepositar = true;
                    }
                    // Se clicou em algo que N�O � cesto enquanto carrega lixo, ignore o clique
                    // O peixe continua parado com o lixo, esperando um clique em um cesto.
                }
                // Se clicou em um lugar vazio enquanto carrega lixo, ignore o clique.
            }
        }

        // Move o jogador
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, velocidadeMovimento * Time.deltaTime);

        // L�gica para quando o jogador alcan�a o alvo
        // Usamos uma pequena margem (0.1f) para considerar que "chegou"
        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            // Se estava indo coletar um lixo espec�fico e chegou perto dele
            if (estaMovendoParaColetar && lixoAlvoParaColetar != null)
            {
                // Apenas se o lixo-alvo for o mesmo que o peixe est� indo coletar
                if (lixoAlvoParaColetar.gameObject.activeInHierarchy && lixoCarregado == null) // Garante que o lixo ainda est� ativo e n�o foi coletado por outro meio
                {
                    lixoCarregado = lixoAlvoParaColetar; // Define que est� carregando ESTE lixo
                    lixoCarregado.Coletar(this.transform);
                    // N�O resetamos targetPosition aqui. Ela se mant�m como a posi��o do lixo,
                    // e o pr�ximo clique do jogador levar� o peixe e o lixo para o cesto.
                }
                estaMovendoParaColetar = false; // Reseta a flag ap�s a tentativa de coleta
                lixoAlvoParaColetar = null; // Limpa o alvo ap�s a tentativa
            }
            // A flag estaMovendoParaDepositar � resetada ap�s o clique no cesto, n�o ao chegar.
            // A l�gica de dep�sito � tratada no CestoLixo via OnTriggerEnter2D.
        }

        // Se o jogador est� carregando lixo, o lixo o seguir� automaticamente.
        if (lixoCarregado != null)
        {
            lixoCarregado.transform.localPosition = Vector3.zero;
        }

        // Virar o jogador (peixinho) para a dire��o do movimento
        if (targetPosition.x < transform.position.x)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
        else if (targetPosition.x > transform.position.x)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
    }

    // M�todo p�blico para ser chamado pelo CestoLixo quando o lixo � depositado com sucesso
    public void LixoDepositado()
    {
        lixoCarregado = null; // O jogador n�o est� mais carregando lixo
        targetPosition = transform.position; // Reseta targetPosition para a posi��o atual do peixe
        // Garante que todas as flags de movimento estejam limpas
        estaMovendoParaColetar = false;
        estaMovendoParaDepositar = false;
        lixoAlvoParaColetar = null;
    }

    public TipoLixo GetTipoLixoCarregado()
    {
        if (lixoCarregado != null)
        {
            return lixoCarregado.tipoLixo;
        }
        return TipoLixo.Nenhum;
    }
}
