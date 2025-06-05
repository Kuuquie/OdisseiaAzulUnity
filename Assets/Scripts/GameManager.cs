using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public enum TipoLixo { Nenhum, Plastico, Metal, Papel, Vidro }

[System.Serializable]
public class LixoPrefab
{
    public GameObject prefab; // O GameObject Prefab do lixo
    public TipoLixo tipo;     // O tipo de lixo associado a este prefab
}

public class GameManager : MonoBehaviour
{
    // Padrão Singleton: Permite que outros scripts acessem esta instância facilmente
    public static GameManager Instance { get; private set; }

    public LixoPrefab[] tiposDeLixoPrefabs; // Array para os diferentes prefabs de lixo e seus tipos
    public int quantidadeTotalLixo = 10; // Total de lixos a serem gerados neste nível

    // NOVO: Array de GameObjects que são os pontos de spawn (definidos no editor)
    public GameObject[] spawnPoints;

    // NOVO: Adicione uma referência ao AudioClip da música de fundo
    public AudioClip musicaDeFundo;
    // NOVO: Adicione uma referência ao AudioClip para o som de nível concluído
    public AudioClip somNivelConcluido;

    public TextMeshProUGUI contadorLixoMensagemTexto; // Referência ao objeto de texto para exibir a contagem
    private int lixoColetadoAtual = 0; // Quantidade de lixo já coletada e depositada

    // Evento para notificar quando o nível é concluído
    public static event System.Action OnNivelConcluido;

    void Awake()
    {
        // Implementação do Singleton: Garante que só exista uma instância deste gerenciador
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Destrói se já existe outra instância
        }
        else
        {
            Instance = this; // Define esta como a única instância
        }
    }

    void Start()
    {
        // NOVO: Verifica se há pontos de spawn configurados
        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogError("Nenhum ponto de spawn de lixo configurado no GameManager! Por favor, arraste os GameObjects dos pontos de spawn para o array 'Spawn Points' no Inspector.");
            return; // Interrompe se não houver pontos
        }

        SpawnarTodosOsLixos(); // Começa gerando os lixos
        AtualizarContador();   // Atualiza o contador na tela

        // NOVO: Toca a música de fundo quando o jogo inicia
        if (AudioManager.Instance != null && musicaDeFundo != null)
        {
            AudioManager.Instance.PlayMusic(musicaDeFundo);
        }
    }

    // Função para gerar todos os lixos aleatoriamente nos pontos de spawn
    void SpawnarTodosOsLixos()
    {
        // NOVO: Cria uma lista temporária de índices disponíveis dos pontos de spawn
        List<int> availableSpawnPoints = new List<int>();
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            availableSpawnPoints.Add(i);
        }

        // Garante que não tentaremos spawnar mais lixos do que pontos disponíveis
        int lixosParaSpawnar = Mathf.Min(quantidadeTotalLixo, availableSpawnPoints.Count);

        for (int i = 0; i < lixosParaSpawnar; i++)
        {
            // Escolhe um ponto de spawn aleatório e remove-o da lista para não repetir
            int randomIndex = Random.Range(0, availableSpawnPoints.Count);
            int spawnPointIndex = availableSpawnPoints[randomIndex];
            availableSpawnPoints.RemoveAt(randomIndex);

            Vector3 spawnPosition = spawnPoints[spawnPointIndex].transform.position;

            // Escolhe um tipo de lixo aleatoriamente do array
            LixoPrefab lixoEscolhido = tiposDeLixoPrefabs[Random.Range(0, tiposDeLixoPrefabs.Length)];

            GameObject novoLixo = Instantiate(lixoEscolhido.prefab, spawnPosition, Quaternion.identity);

            // Adiciona o script ItemLixo ao GameObject recém-instanciado se ele ainda não tiver um
            TrashItem itemLixoComponent = novoLixo.GetComponent<TrashItem>();
            if (itemLixoComponent == null)
            {
                itemLixoComponent = novoLixo.AddComponent<TrashItem>();
            }
            // Atribui o tipo correto ao ItemLixo instanciado
            itemLixoComponent.tipoLixo = lixoEscolhido.tipo;
        }
    }

    // Método público para ser chamado pelo ItemLixo quando ele é depositado corretamente
    public void LixoColetado()
    {
        lixoColetadoAtual++; // Incrementa a contagem de lixo coletado
        AtualizarContador();   // Atualiza o texto na tela

        // Verifica se todo o lixo necessário foi coletado
        if (lixoColetadoAtual >= quantidadeTotalLixo)
        {
            // Dispara o evento de nível concluído (outros scripts podem estar "ouvindo")
            if (OnNivelConcluido != null)
            {
                OnNivelConcluido();
            }
            NivelConcluido(); // Chama a função para lidar com a conclusão do nível
        }
    }

    // Atualiza o texto do contador na tela
    void AtualizarContador()
    {
        if (contadorLixoMensagemTexto != null)
        {
            contadorLixoMensagemTexto.text = "Lixo: " + lixoColetadoAtual + "/" + quantidadeTotalLixo;
            contadorLixoMensagemTexto.gameObject.SetActive(true); // Garante que o texto esteja visível
        }
    }

    // Lógica para quando o nível é concluído
    void NivelConcluido()
    {
        if (contadorLixoMensagemTexto != null)
        {
            contadorLixoMensagemTexto.text = "Nível de Limpeza Concluído!";
            Debug.Log("Nível Concluído!"); // Mensagem no console da Unity
        }
        // NOVO: Toca um som quando o nível é concluído
        if (AudioManager.Instance != null && somNivelConcluido != null)
        {
            AudioManager.Instance.Play(somNivelConcluido);
            // Opcional: Parar a música de fundo ao concluir o nível
            // AudioManager.Instance.StopMusic(); 
        }
        // *** Aqui você pode adicionar lógica para carregar o próximo nível ou ativar a tela de puzzle do coral! ***
        // Ex: SceneManager.LoadScene("NomeDaCenaDoPuzzleDoCoral");
        // Ou ativar um painel de UI que inicie o puzzle.
    }
}
