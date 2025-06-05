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

    // NOVO: Adiciona uma referência ao lixo que o peixe *está indo buscar*
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

            // Reseta flags de movimento para garantir que novas ações não fiquem presas em estados antigos
            estaMovendoParaColetar = false;
            estaMovendoParaDepositar = false;
            lixoAlvoParaColetar = null; // Limpa o alvo de coleta no início de um novo clique

            if (lixoCarregado == null) // O jogador NÃO está carregando lixo
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
                    else // Clicou em algo que não é lixo (ou é um cesto vazio)
                    {
                        targetPosition = clickPosition; // Move livremente
                    }
                }
                else // Clicou em um lugar vazio
                {
                    targetPosition = clickPosition; // Move livremente
                }
            }
            else // O jogador ESTÁ carregando lixo
            {
                if (hit.collider != null)
                {
                    TrashBin cestoClicado = hit.collider.GetComponent<TrashBin>();

                    if (cestoClicado != null) // Clicou em um cesto
                    {
                        targetPosition = cestoClicado.transform.position;
                        estaMovendoParaDepositar = true;
                    }
                    // Se clicou em algo que NÃO É cesto enquanto carrega lixo, ignore o clique
                    // O peixe continua parado com o lixo, esperando um clique em um cesto.
                }
                // Se clicou em um lugar vazio enquanto carrega lixo, ignore o clique.
            }
        }

        // Move o jogador
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, velocidadeMovimento * Time.deltaTime);

        // Lógica para quando o jogador alcança o alvo
        // Usamos uma pequena margem (0.1f) para considerar que "chegou"
        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            // Se estava indo coletar um lixo específico e chegou perto dele
            if (estaMovendoParaColetar && lixoAlvoParaColetar != null)
            {
                // Apenas se o lixo-alvo for o mesmo que o peixe está indo coletar
                if (lixoAlvoParaColetar.gameObject.activeInHierarchy && lixoCarregado == null) // Garante que o lixo ainda está ativo e não foi coletado por outro meio
                {
                    lixoCarregado = lixoAlvoParaColetar; // Define que está carregando ESTE lixo
                    lixoCarregado.Coletar(this.transform);
                    // NÃO resetamos targetPosition aqui. Ela se mantém como a posição do lixo,
                    // e o próximo clique do jogador levará o peixe e o lixo para o cesto.
                }
                estaMovendoParaColetar = false; // Reseta a flag após a tentativa de coleta
                lixoAlvoParaColetar = null; // Limpa o alvo após a tentativa
            }
            // A flag estaMovendoParaDepositar é resetada após o clique no cesto, não ao chegar.
            // A lógica de depósito é tratada no CestoLixo via OnTriggerEnter2D.
        }

        // Se o jogador está carregando lixo, o lixo o seguirá automaticamente.
        if (lixoCarregado != null)
        {
            lixoCarregado.transform.localPosition = Vector3.zero;
        }

        // Virar o jogador (peixinho) para a direção do movimento
        if (targetPosition.x < transform.position.x)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
        else if (targetPosition.x > transform.position.x)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
    }

    // Método público para ser chamado pelo CestoLixo quando o lixo é depositado com sucesso
    public void LixoDepositado()
    {
        lixoCarregado = null; // O jogador não está mais carregando lixo
        targetPosition = transform.position; // Reseta targetPosition para a posição atual do peixe
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
