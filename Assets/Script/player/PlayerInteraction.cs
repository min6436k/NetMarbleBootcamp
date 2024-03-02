using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public enum InteractionType
{
    NONE,
    TREE,
    PLANT,
    STONE,
    POTAL
}

public class PlayerInteraction : MonoBehaviour
{

    private Vector3[] DIRECTIONS = { Vector3.zero, Vector3.left, Vector3.right, Vector3.up, Vector3.down };
    private Vector3[] OFFSETS = { new Vector3(), new Vector3(-0.35F, 1.25F), new Vector3(0.35F, 1.25F), new Vector3(0, 1.75F), new Vector3(0, 0.5F)};

    protected GameObject PlayerObject;
    protected player PlayerController;

    private List<RaycastHit2D> HitResults = new List<RaycastHit2D>();
    private GameObject LastInteractableObject;

    private GameManager GameManagerScript;
    private Button InteractionButton;

    private float InteractionTime;

    [Header("설정")]
    public GameObject GameManagerObject;

    void Start()
    {
        PlayerObject = GameObject.FindGameObjectWithTag("Player");
        if (PlayerObject == null)
            throw new Exception("현재 Scene에서 플레이어를 찾을 수 없습니다.");
        PlayerController = PlayerObject.GetComponent<player>();
        if (PlayerController == null)
            throw new Exception("Player 태그를 가진 게임오브젝트가 잘못되었습니다.");
        if (GameManagerObject == null)
            throw new Exception("GameManager를 선택해주세요.");
        GameManagerScript = GameManagerObject.GetComponent<GameManager>();
        if (GameManagerScript == null)
            throw new Exception("GameManager 스크립트를 찾을 수 없습니다.");
        InteractionButton = GameManagerScript.UIInteraction.GetComponent<Button>();
    }

    void Update()
    {
        GameObject InteractableObject = FindInteractable();
        if (InteractableObject != LastInteractableObject)
        {
            OnExitInteractableObject(LastInteractableObject);
            OnEnterInteractableObject(InteractableObject);
        }
        else
        {
            OnStayInteractableObject(InteractableObject);
        }
        LastInteractableObject = InteractableObject;

        if (InteractableObject == null)
        {
            InteractionButton.interactable = false;
        }
        else
        {
            InteractionButton.interactable = true;
        }
    }

    public void OnEnterInteractableObject(GameObject Object)
    {
        if (Object == null) return;
        InteractionTime = 0;
    }

    public void OnStayInteractableObject(GameObject Object)
    {
        if (Object == null) return;

        InteractionTime += Time.deltaTime * 2F;

        SpriteOutline OutlineScript = Object.GetComponent<SpriteOutline>();
        if (OutlineScript != null)
        {
            float Alpha = Mathf.Min(Mathf.Abs(Mathf.Max(Mathf.Sin(InteractionTime), -1.0F)), 1.0F);
            OutlineScript.color = new Color(1, 1, 1, Alpha);
            OutlineScript.outlineSize = 1;
        }
    }

    public void OnExitInteractableObject(GameObject Object)
    {
        if (Object == null) return;

        SpriteOutline OutlineScript = Object.GetComponent<SpriteOutline>();
        if (OutlineScript != null)
        {
            OutlineScript.outlineSize = 0;
        }
    }

    public void OnInteraction(GameObject Object, InteractionType Type)
    {
        if (Type == InteractionType.POTAL)
        {
            switch (Object.name)
            {
                case "Go_Lobby": SceneManager.LoadScene("Lobby"); break;
                case "Go_Stage": SceneManager.LoadScene("Nomal1"); break;
                //스테이지
                case "N1_next": SceneManager.LoadScene("Nomal2"); break;
                case "N2_next": SceneManager.LoadScene("NomalBoss"); break;
                case "N1_prev": SceneManager.LoadScene("Lobby"); break;
                case "N2_prev": SceneManager.LoadScene("Nomal1"); break;
                case "N_B_prev": SceneManager.LoadScene("Nomal2"); break;
                //로비
                case "Go_CookingRoom": SceneManager.LoadScene("CookingRoom"); break;
            }
        }
    }

    public void OnInteraction()
    {
        if (LastInteractableObject == null)
            return;

        InteractionType Type = InteractionType.NONE;
        if (LastInteractableObject.name.StartsWith("Tree"))
            Type = InteractionType.TREE;
        else if (LastInteractableObject.name.StartsWith("Plant"))
            Type = InteractionType.PLANT;
        else if (LastInteractableObject.name.StartsWith("Stone"))
            Type = InteractionType.STONE;

        if (Type == InteractionType.NONE)
            Debug.Log("상호작용된 게임 오브젝트의 타입을 추론할 수 없습니다: " + LastInteractableObject.name);
        else
            OnInteraction(LastInteractableObject, Type);
    }

    public GameObject FindInteractable()
    {
        HitResults.Clear();

        RaycastHit2D Raycast;
        for (int i = 0; i < DIRECTIONS.Length; i++)
        {
            Vector3 Direction = DIRECTIONS[i];
            Vector3 Offset = OFFSETS[i];
            Raycast = Raycasting(Direction, Offset);
            if (Raycast.collider != null)
                HitResults.Add(Raycast);
        }

        float Distance = float.MaxValue, TempDistance;
        Raycast = new RaycastHit2D();

        foreach (RaycastHit2D Result in HitResults)
        {
            TempDistance = Vector3.Distance(transform.position, Result.collider.transform.position);
            if (TempDistance < Distance)
            {
                Distance = TempDistance;
                Raycast = Result;
            }
        }

        return Raycast.collider != null ? Raycast.collider.gameObject : null;
    }

    public RaycastHit2D Raycasting(Vector3 Direction, Vector3 Offset)
    {
        Debug.DrawRay(transform.position + Offset, Direction, Color.red);
        return Physics2D.Raycast(transform.position + Offset, Direction, 1.0F, LayerMask.GetMask("Interactable"));
    }

}
