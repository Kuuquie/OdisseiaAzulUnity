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
    public GameObject[] spawnPoints;

    public AudioClip musicaDeFundo;
    public AudioClip somNivelConcluido;

    public TextMeshProUGUI contadorLixoMensagemTexto;
    private int lixoColetadoAtual = 0;

    public static event System.Action OnNivelConcluido;

    public SpriteRenderer fundoOceanoSpriteRenderer; 
    public Sprite fundoSujoSprite;                   
    public Sprite fundoMeioSujoSprite;               
    public Sprite fundoLimpoSprite;                  

    public int lixosParaMeioSujo = 5;

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
        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogError("Nenhum ponto de spawn de lixo configurado no GameManager!");
            return;
        }

        if (fundoOceanoSpriteRenderer == null)
        {
            Debug.LogError("SpriteRenderer do Fundo do Oceano não atribuído no GameManager! Por favor, arraste o GameObject do fundo.");
            return;
        }

        fundoOceanoSpriteRenderer.sprite = fundoSujoSprite;

        SpawnarTodosOsLixos();
        AtualizarContador();

        if (AudioManager.Instance != null && musicaDeFundo != null)
        {
            AudioManager.Instance.PlayMusic(musicaDeFundo);
        }
    }

    void SpawnarTodosOsLixos()
    {
        List<int> availableSpawnPoints = new List<int>();
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            availableSpawnPoints.Add(i);
        }

        int lixosParaSpawnar = Mathf.Min(quantidadeTotalLixo, availableSpawnPoints.Count);

        for (int i = 0; i < lixosParaSpawnar; i++)
        {
            int randomIndex = Random.Range(0, availableSpawnPoints.Count);
            int spawnPointIndex = availableSpawnPoints[randomIndex];
            availableSpawnPoints.RemoveAt(randomIndex);

            Vector3 spawnPosition = spawnPoints[spawnPointIndex].transform.position;

            LixoPrefab lixoEscolhido = tiposDeLixoPrefabs[Random.Range(0, tiposDeLixoPrefabs.Length)];

            GameObject novoLixo = Instantiate(lixoEscolhido.prefab, spawnPosition, Quaternion.identity);

            TrashItem itemLixoComponent = novoLixo.GetComponent<TrashItem>();
            if (itemLixoComponent == null)
            {
                itemLixoComponent = novoLixo.AddComponent<TrashItem>();
            }
            itemLixoComponent.tipoLixo = lixoEscolhido.tipo;
        }
    }

    public void LixoColetado()
    {
        lixoColetadoAtual++;
        AtualizarContador();

       
        if (lixoColetadoAtual >= quantidadeTotalLixo)
        {
            
            if (fundoOceanoSpriteRenderer != null && fundoLimpoSprite != null)
            {
                fundoOceanoSpriteRenderer.sprite = fundoLimpoSprite;
            }
            if (OnNivelConcluido != null)
            {
                OnNivelConcluido();
            }
            NivelConcluido();
        }
        else if (lixoColetadoAtual >= lixosParaMeioSujo)
        {
        
            if (fundoOceanoSpriteRenderer != null && fundoMeioSujoSprite != null)
            {
                fundoOceanoSpriteRenderer.sprite = fundoMeioSujoSprite;
            }
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
            contadorLixoMensagemTexto.text = "Nível de Limpeza Concluído!";
            Debug.Log("Nível Concluído!");
        }
        if (AudioManager.Instance != null && somNivelConcluido != null)
        {
            AudioManager.Instance.Play(somNivelConcluido);
        }
    }
}
