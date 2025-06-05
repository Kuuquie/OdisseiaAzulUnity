using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public enum TipoLixo { Nenhum, Plastico, Metal, Papel, Vidro }

[System.Serializable]
public class LixoPrefab
{
    public GameObject prefab;
    public TipoLixo tipo;
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public LixoPrefab[] tiposDeLixoPrefabs;
    public int quantidadeTotalLixo = 10;

    // NOVO: Lista de GameObjects que são os pontos de spawn (definidos no editor)
    public GameObject[] spawnPoints;

    public TextMeshProUGUI contadorLixoMensagemTexto;
    private int lixoColetadoAtual = 0;

    public static event System.Action OnNivelConcluido;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
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

        SpawnarTodosOsLixos();
        AtualizarContador();
    }

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

            // Escolhe um tipo de lixo aleatoriamente
            LixoPrefab lixoEscolhido = tiposDeLixoPrefabs[Random.Range(0, tiposDeLixoPrefabs.Length)];

            GameObject novoLixo = Instantiate(lixoEscolhido.prefab, spawnPosition, Quaternion.identity);

            TrashItem itemLixoComponent = novoLixo.GetComponent<TrashItem>();
            if (itemLixoComponent == null)
            {
                itemLixoComponent = novoLixo.AddComponent<TrashItem>();
            }
            itemLixoComponent.tipoLixo = lixoEscolhido.tipo;

            // Removido: A lógica de OverlapCircle não é mais necessária aqui,
            // pois os pontos já são definidos para serem livres.
        }
    }

    public void LixoColetado()
    {
        lixoColetadoAtual++;
        AtualizarContador();

        if (lixoColetadoAtual >= quantidadeTotalLixo)
        {
            if (OnNivelConcluido != null)
            {
                OnNivelConcluido();
            }
            NivelConcluido();
        }
    }

    void AtualizarContador()
    {
        if (contadorLixoMensagemTexto != null)
        {
            contadorLixoMensagemTexto.text = "Lixo: " + lixoColetadoAtual + "/" + quantidadeTotalLixo;
            contadorLixoMensagemTexto.gameObject.SetActive(true);
        }
    }

    void NivelConcluido()
    {
        if (contadorLixoMensagemTexto != null)
        {
            contadorLixoMensagemTexto.text = "Nível de Limpeza Concluido!";
            Debug.Log("Nível Concluído!");
        }
    }

}

