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
    // Padr�o Singleton: Permite que outros scripts acessem esta inst�ncia facilmente
    public static GameManager Instance { get; private set; }

    public LixoPrefab[] tiposDeLixoPrefabs; // Array para os diferentes prefabs de lixo e seus tipos
    public int quantidadeTotalLixo = 10; // Total de lixos a serem gerados neste n�vel

    // NOVO: Array de GameObjects que s�o os pontos de spawn (definidos no editor)
    public GameObject[] spawnPoints;

    // NOVO: Adicione uma refer�ncia ao AudioClip da m�sica de fundo
    public AudioClip musicaDeFundo;
    // NOVO: Adicione uma refer�ncia ao AudioClip para o som de n�vel conclu�do
    public AudioClip somNivelConcluido;

    public TextMeshProUGUI contadorLixoMensagemTexto; // Refer�ncia ao objeto de texto para exibir a contagem
    private int lixoColetadoAtual = 0; // Quantidade de lixo j� coletada e depositada

    // Evento para notificar quando o n�vel � conclu�do
    public static event System.Action OnNivelConcluido;

    void Awake()
    {
        // Implementa��o do Singleton: Garante que s� exista uma inst�ncia deste gerenciador
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Destr�i se j� existe outra inst�ncia
        }
        else
        {
            Instance = this; // Define esta como a �nica inst�ncia
        }
    }

    void Start()
    {
        // NOVO: Verifica se h� pontos de spawn configurados
        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogError("Nenhum ponto de spawn de lixo configurado no GameManager! Por favor, arraste os GameObjects dos pontos de spawn para o array 'Spawn Points' no Inspector.");
            return; // Interrompe se n�o houver pontos
        }

        SpawnarTodosOsLixos(); // Come�a gerando os lixos
        AtualizarContador();   // Atualiza o contador na tela

        // NOVO: Toca a m�sica de fundo quando o jogo inicia
        if (AudioManager.Instance != null && musicaDeFundo != null)
        {
            AudioManager.Instance.PlayMusic(musicaDeFundo);
        }
    }

    // Fun��o para gerar todos os lixos aleatoriamente nos pontos de spawn
    void SpawnarTodosOsLixos()
    {
        // NOVO: Cria uma lista tempor�ria de �ndices dispon�veis dos pontos de spawn
        List<int> availableSpawnPoints = new List<int>();
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            availableSpawnPoints.Add(i);
        }

        // Garante que n�o tentaremos spawnar mais lixos do que pontos dispon�veis
        int lixosParaSpawnar = Mathf.Min(quantidadeTotalLixo, availableSpawnPoints.Count);

        for (int i = 0; i < lixosParaSpawnar; i++)
        {
            // Escolhe um ponto de spawn aleat�rio e remove-o da lista para n�o repetir
            int randomIndex = Random.Range(0, availableSpawnPoints.Count);
            int spawnPointIndex = availableSpawnPoints[randomIndex];
            availableSpawnPoints.RemoveAt(randomIndex);

            Vector3 spawnPosition = spawnPoints[spawnPointIndex].transform.position;

            // Escolhe um tipo de lixo aleatoriamente do array
            LixoPrefab lixoEscolhido = tiposDeLixoPrefabs[Random.Range(0, tiposDeLixoPrefabs.Length)];

            GameObject novoLixo = Instantiate(lixoEscolhido.prefab, spawnPosition, Quaternion.identity);

            // Adiciona o script ItemLixo ao GameObject rec�m-instanciado se ele ainda n�o tiver um
            TrashItem itemLixoComponent = novoLixo.GetComponent<TrashItem>();
            if (itemLixoComponent == null)
            {
                itemLixoComponent = novoLixo.AddComponent<TrashItem>();
            }
            // Atribui o tipo correto ao ItemLixo instanciado
            itemLixoComponent.tipoLixo = lixoEscolhido.tipo;
        }
    }

    // M�todo p�blico para ser chamado pelo ItemLixo quando ele � depositado corretamente
    public void LixoColetado()
    {
        lixoColetadoAtual++; // Incrementa a contagem de lixo coletado
        AtualizarContador();   // Atualiza o texto na tela

        // Verifica se todo o lixo necess�rio foi coletado
        if (lixoColetadoAtual >= quantidadeTotalLixo)
        {
            // Dispara o evento de n�vel conclu�do (outros scripts podem estar "ouvindo")
            if (OnNivelConcluido != null)
            {
                OnNivelConcluido();
            }
            NivelConcluido(); // Chama a fun��o para lidar com a conclus�o do n�vel
        }
    }

    // Atualiza o texto do contador na tela
    void AtualizarContador()
    {
        if (contadorLixoMensagemTexto != null)
        {
            contadorLixoMensagemTexto.text = "Lixo: " + lixoColetadoAtual + "/" + quantidadeTotalLixo;
            contadorLixoMensagemTexto.gameObject.SetActive(true); // Garante que o texto esteja vis�vel
        }
    }

    // L�gica para quando o n�vel � conclu�do
    void NivelConcluido()
    {
        if (contadorLixoMensagemTexto != null)
        {
            contadorLixoMensagemTexto.text = "N�vel de Limpeza Conclu�do!";
            Debug.Log("N�vel Conclu�do!"); // Mensagem no console da Unity
        }
        // NOVO: Toca um som quando o n�vel � conclu�do
        if (AudioManager.Instance != null && somNivelConcluido != null)
        {
            AudioManager.Instance.Play(somNivelConcluido);
            // Opcional: Parar a m�sica de fundo ao concluir o n�vel
            // AudioManager.Instance.StopMusic(); 
        }
        // *** Aqui voc� pode adicionar l�gica para carregar o pr�ximo n�vel ou ativar a tela de puzzle do coral! ***
        // Ex: SceneManager.LoadScene("NomeDaCenaDoPuzzleDoCoral");
        // Ou ativar um painel de UI que inicie o puzzle.
    }
}
