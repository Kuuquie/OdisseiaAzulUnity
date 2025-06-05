using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float velocidadeMovimento = 5.0f; // Velocidade de movimento do jogador (peixinho)
    private Vector3 targetPosition;         // Posição para onde o jogador deve ir
    private TrashItem lixoCarregado = null;  // Referência ao lixo que o jogador está carregando
    private bool estaMovendoParaColetar = false; // Flag para saber se o jogador está indo pegar lixo
    private bool estaMovendoParaDepositar = false; // Flag para saber se o jogador está indo depositar lixo

    // NOVO: Adiciona uma referência ao lixo que o peixe *está indo buscar*
    private TrashItem lixoAlvoParaColetar = null;

    void Start()
    {
        targetPosition = transform.position; // O jogador começa na sua posição atual
    }

    void Update()
    {
        // Detecta o clique do mouse
        if (Input.GetMouseButtonDown(0)) // 0 para o botão esquerdo do mouse
        {
            Vector3 clickPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            clickPosition.z = transform.position.z; // Manter a profundidade Z do jogador

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
                    // CestoLixo cestoClicado = hit.collider.GetComponent<CestoLixo>(); // Não usado diretamente aqui, mas pode ser para lógica futura

                    if (lixoClicado != null) // Clicou em um lixo
                    {
                        targetPosition = lixoClicado.transform.position;
                        lixoAlvoParaColetar = lixoClicado; // Define o lixo alvo que ele irá buscar
                        estaMovendoParaColetar = true;
                    }
                    else // Clicou em algo que não é lixo (ou é um cesto vazio, ou fundo)
                    {
                        targetPosition = clickPosition; // Move livremente para a posição do clique
                    }
                }
                else // Clicou em um lugar vazio (não atingiu nenhum collider)
                {
                    targetPosition = clickPosition; // Move livremente para a posição do clique
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
                    // Se clicou em algo que NÃO É cesto enquanto carrega lixo, ignore o clique.
                    // O peixe continua parado com o lixo, esperando um clique em um cesto.
                }
                // Se clicou em um lugar vazio enquanto carrega lixo, ignore o clique.
            }
        }

        // Move o jogador em direção à targetPosition
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, velocidadeMovimento * Time.deltaTime);

        // Lógica para quando o jogador alcança o alvo
        // Usamos uma pequena margem (0.1f) para considerar que "chegou"
        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            // Se estava indo coletar um lixo específico e chegou perto dele
            if (estaMovendoParaColetar && lixoAlvoParaColetar != null)
            {
                // Apenas se o lixo-alvo for o mesmo que o peixe está indo coletar
                // e o lixo ainda está ativo na hierarquia e o peixe não está carregando nada
                if (lixoAlvoParaColetar.gameObject.activeInHierarchy && lixoCarregado == null)
                {
                    lixoCarregado = lixoAlvoParaColetar; // Define que está carregando ESTE lixo
                    lixoCarregado.Coletar(this.transform); // Chama a função Coletar do ItemLixo
                }
                estaMovendoParaColetar = false; // Reseta a flag após a tentativa de coleta
                lixoAlvoParaColetar = null; // Limpa o alvo após a tentativa
            }
            // A lógica de depósito é tratada no CestoLixo via OnTriggerEnter2D,
            // então não precisamos de uma lógica de "chegou ao cesto" aqui diretamente.
        }

        // Se o jogador está carregando lixo, o lixo o seguirá automaticamente porque é seu filho.
        if (lixoCarregado != null)
        {
            lixoCarregado.transform.localPosition = Vector3.zero; // Ajusta a posição local do lixo para o peixe
        }

        // Opcional: Virar o jogador (peixinho) para a direção do movimento
        if (targetPosition.x < transform.position.x)
        {
            transform.localScale = new Vector3(-1, 1, 1); // Vira para a esquerda
        }
        else if (targetPosition.x > transform.position.x)
        {
            transform.localScale = new Vector3(1, 1, 1); // Vira para a direita
        }
    }

    // Método público para ser chamado pelo CestoLixo quando o lixo é depositado com sucesso
    public void LixoDepositado()
    {
        lixoCarregado = null; // O jogador não está mais carregando lixo
        targetPosition = transform.position; // Reseta targetPosition para a posição atual do peixe
        // Garante que todas as flags de movimento e alvos estejam limpos
        estaMovendoParaColetar = false;
        estaMovendoParaDepositar = false;
        lixoAlvoParaColetar = null;
    }

    // Método público para retornar o tipo de lixo que o jogador está carregando
    public TipoLixo GetTipoLixoCarregado()
    {
        if (lixoCarregado != null)
        {
            return lixoCarregado.tipoLixo;
        }
        return TipoLixo.Nenhum; // Retorna TipoLixo.Nenhum se o jogador não estiver carregando lixo
    }
}
