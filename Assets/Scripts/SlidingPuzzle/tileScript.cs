using UnityEngine;

public class tileScript : MonoBehaviour
{
    public Vector3 targetPosition;

    private Vector3 correctPosition;
    private SpriteRenderer _sprite;

    public int number;

    private void Awake()
    {
        _sprite = GetComponent<SpriteRenderer>();

        // Força a posição inicial para Z = 0
        Vector3 posicao = transform.position;
        posicao.z = 0f;

        transform.position = posicao;

        targetPosition = posicao;
        correctPosition = posicao;
    }

    private void Update()
    {
        transform.position = Vector3.Lerp(
            transform.position,
            targetPosition,
            0.08f
        );

        // Quando chegar perto o suficiente,
        // coloca exatamente na posição desejada.
        if (Vector3.Distance(transform.position, targetPosition) < 0.001f)
        {
            transform.position = targetPosition;
        }

        if (targetPosition == correctPosition)
        {
            _sprite.color = Color.green;
        }
        else
        {
            _sprite.color = Color.white;
        }
    }
}