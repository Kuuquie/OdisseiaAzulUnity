using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float velocidadeMovimento = 5.0f; // Velocidade de movimento do jogador (peixinho)
    private Vector3 targetPosition;         // Posi��o para onde o jogador deve ir
    private TrashItem lixoCarregado = null;  // Refer�ncia ao lixo que o jogador est� carregando
    private bool estaMovendoParaColetar = false; // Flag para saber se o jogador est� indo pegar lixo
    private bool estaMovendoParaDepositar = false; // Flag para saber se o jogador est� indo depositar lixo

    // NOVO: Adiciona uma refer�ncia ao lixo que o peixe *est� indo buscar*
    private TrashItem lixoAlvoParaColetar = null;

    void Start()
    {
        targetPosition = transform.position; // O jogador come�a na sua posi��o atual
    }

    void Update()
    {
        // Detecta o clique do mouse
        if (Input.GetMouseButtonDown(0)) // 0 para o bot�o esquerdo do mouse
        {
            Vector3 clickPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            clickPosition.z = transform.position.z; // Manter a profundidade Z do jogador

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
                    // CestoLixo cestoClicado = hit.collider.GetComponent<CestoLixo>(); // N�o usado diretamente aqui, mas pode ser para l�gica futura

                    if (lixoClicado != null) // Clicou em um lixo
                    {
                        targetPosition = lixoClicado.transform.position;
                        lixoAlvoParaColetar = lixoClicado; // Define o lixo alvo que ele ir� buscar
                        estaMovendoParaColetar = true;
                    }
                    else // Clicou em algo que n�o � lixo (ou � um cesto vazio, ou fundo)
                    {
                        targetPosition = clickPosition; // Move livremente para a posi��o do clique
                    }
                }
                else // Clicou em um lugar vazio (n�o atingiu nenhum collider)
                {
                    targetPosition = clickPosition; // Move livremente para a posi��o do clique
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
                    // Se clicou em algo que N�O � cesto enquanto carrega lixo, ignore o clique.
                    // O peixe continua parado com o lixo, esperando um clique em um cesto.
                }
                // Se clicou em um lugar vazio enquanto carrega lixo, ignore o clique.
            }
        }

        // Move o jogador em dire��o � targetPosition
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, velocidadeMovimento * Time.deltaTime);

        // L�gica para quando o jogador alcan�a o alvo
        // Usamos uma pequena margem (0.1f) para considerar que "chegou"
        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            // Se estava indo coletar um lixo espec�fico e chegou perto dele
            if (estaMovendoParaColetar && lixoAlvoParaColetar != null)
            {
                // Apenas se o lixo-alvo for o mesmo que o peixe est� indo coletar
                // e o lixo ainda est� ativo na hierarquia e o peixe n�o est� carregando nada
                if (lixoAlvoParaColetar.gameObject.activeInHierarchy && lixoCarregado == null)
                {
                    lixoCarregado = lixoAlvoParaColetar; // Define que est� carregando ESTE lixo
                    lixoCarregado.Coletar(this.transform); // Chama a fun��o Coletar do ItemLixo
                }
                estaMovendoParaColetar = false; // Reseta a flag ap�s a tentativa de coleta
                lixoAlvoParaColetar = null; // Limpa o alvo ap�s a tentativa
            }
            // A l�gica de dep�sito � tratada no CestoLixo via OnTriggerEnter2D,
            // ent�o n�o precisamos de uma l�gica de "chegou ao cesto" aqui diretamente.
        }

        // Se o jogador est� carregando lixo, o lixo o seguir� automaticamente porque � seu filho.
        if (lixoCarregado != null)
        {
            lixoCarregado.transform.localPosition = Vector3.zero; // Ajusta a posi��o local do lixo para o peixe
        }

        // Opcional: Virar o jogador (peixinho) para a dire��o do movimento
        if (targetPosition.x < transform.position.x)
        {
            transform.localScale = new Vector3(-1, 1, 1); // Vira para a esquerda
        }
        else if (targetPosition.x > transform.position.x)
        {
            transform.localScale = new Vector3(1, 1, 1); // Vira para a direita
        }
    }

    // M�todo p�blico para ser chamado pelo CestoLixo quando o lixo � depositado com sucesso
    public void LixoDepositado()
    {
        lixoCarregado = null; // O jogador n�o est� mais carregando lixo
        targetPosition = transform.position; // Reseta targetPosition para a posi��o atual do peixe
        // Garante que todas as flags de movimento e alvos estejam limpos
        estaMovendoParaColetar = false;
        estaMovendoParaDepositar = false;
        lixoAlvoParaColetar = null;
    }

    // M�todo p�blico para retornar o tipo de lixo que o jogador est� carregando
    public TipoLixo GetTipoLixoCarregado()
    {
        if (lixoCarregado != null)
        {
            return lixoCarregado.tipoLixo;
        }
        return TipoLixo.Nenhum; // Retorna TipoLixo.Nenhum se o jogador n�o estiver carregando lixo
    }
}
