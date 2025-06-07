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

   
    private TrashItem lixoAlvoParaColetar = null;

   
    private TrashBin[] todosOsCestosNaCena;

   
    private Animator[] cestosAnimators;

    void Start()
    {
        targetPosition = transform.position;

        
        todosOsCestosNaCena = FindObjectsOfType<TrashBin>();

        
        cestosAnimators = new Animator[todosOsCestosNaCena.Length];
        for (int i = 0; i < todosOsCestosNaCena.Length; i++)
        {
            cestosAnimators[i] = todosOsCestosNaCena[i].GetComponent<Animator>();
        }
    }

    void Update()
    {
       
        if (Input.GetMouseButtonDown(0))
        {
            
            Vector3 clickPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            clickPosition.z = transform.position.z; 

          
            RaycastHit2D hit = Physics2D.Raycast(clickPosition, Vector2.zero);

            
            estaMovendoParaColetar = false;
            estaMovendoParaDepositar = false;
            lixoAlvoParaColetar = null;

            if (lixoCarregado == null)
            {
                if (hit.collider != null)
                {
                    TrashItem lixoClicado = hit.collider.GetComponent<TrashItem>();

                    if (lixoClicado != null)
                    {
                        targetPosition = lixoClicado.transform.position;
                        lixoAlvoParaColetar = lixoClicado;
                        estaMovendoParaColetar = true;
                    }
                    else
                    {
                        targetPosition = clickPosition;
                    }
                }
                else
                {
                    targetPosition = clickPosition;
                }
            }
            else
            {
                if (hit.collider != null)
                {
                    TrashBin cestoClicado = hit.collider.GetComponent<TrashBin>();

                    if (cestoClicado != null)
                    {
                        targetPosition = cestoClicado.transform.position;
                        estaMovendoParaDepositar = true;
                    }
                   
                }
                
            }
        }

      
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, velocidadeMovimento * Time.deltaTime);

        
        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            
            if (estaMovendoParaColetar && lixoAlvoParaColetar != null)
            {
                
                if (lixoAlvoParaColetar.gameObject.activeInHierarchy && lixoCarregado == null)
                {
                    lixoCarregado = lixoAlvoParaColetar; 
                    lixoCarregado.Coletar(this.transform);

                    
                    AtivarPulsacaoNaLixeiraCorreta(lixoCarregado.tipoLixo);
                }
                estaMovendoParaColetar = false;
                lixoAlvoParaColetar = null;
            }
        }

        
        if (lixoCarregado != null)
        {
            
            lixoCarregado.transform.localPosition = Vector3.zero;
        }

        
        if (targetPosition.x < transform.position.x)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
        else if (targetPosition.x > transform.position.x)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
    }

    
    public void LixoDepositado()
    {
        lixoCarregado = null;
        targetPosition = transform.position;
        estaMovendoParaColetar = false;
        estaMovendoParaDepositar = false;
        lixoAlvoParaColetar = null;

        
        DesativarPulsacaoEmTodasLixeiras();
    }

    
    public TipoLixo GetTipoLixoCarregado()
    {
        if (lixoCarregado != null)
        {
            return lixoCarregado.tipoLixo;
        }
        return TipoLixo.Nenhum; 
    }

   
    void AtivarPulsacaoNaLixeiraCorreta(TipoLixo tipoLixo)
    {
        for (int i = 0; i < todosOsCestosNaCena.Length; i++)
        {
            TrashBin cesto = todosOsCestosNaCena[i];
            Animator animator = cestosAnimators[i]; 

            if (animator != null)
            {
                if (cesto.tipoAceito == tipoLixo) 
                {
                    animator.SetBool("Pulsar", true); 
                }
                else 
                {
                    animator.SetBool("Pulsar", false); 
                }
            }
        }
    }

    void DesativarPulsacaoEmTodasLixeiras()
    {
        
        foreach (Animator animator in cestosAnimators)
        {
            if (animator != null)
            {
                animator.SetBool("Pulsar", false);
            }
        }
    }
}
