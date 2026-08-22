using UnityEngine;
using TMPro;
using System.Collections;
public class ShowMessageUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI statusText;
    [SerializeField] private float displayDuration = 3.0f;
    [SerializeField] private float fadeDuration = 1.0f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(statusText != null)
        {
            statusText.gameObject.SetActive(false);
        }
    }


    public void ShowMessage()
    {
        StopAllCoroutines();
        StartCoroutine(DisplayMessage());
    }

    private IEnumerator DisplayMessage()
    {
        statusText.text = "PUZZLE COMPLETADO";
        statusText.gameObject.SetActive(true);

        SetTextAlpha(1f);

        // 2. Espera o tempo de exibição (usando tempo real, não o da física)
        yield return new WaitForSecondsRealtime(displayDuration);

        // 3. Animação de Fade Out (desaparecer aos poucos)
        float currentTime = 0f;
        while (currentTime < fadeDuration)
        {
            currentTime += Time.unscaledDeltaTime; // Usa tempo real

            // Calcula a porcentagem da animação (de 0 a 1)
            float alpha = Mathf.Lerp(1f, 0f, currentTime / fadeDuration);
            
            // Aplica o novo alpha ao texto
            SetTextAlpha(alpha);

            // Espera até o próximo frame
            yield return null; 
        }

        // 4. Garante que sumiu totalmente e desativa
        SetTextAlpha(0f);

        statusText.gameObject.SetActive(false);
    }

    private void SetTextAlpha(float alpha)
    {
        Color textColor = statusText.color;
        textColor.a = alpha;
        statusText.color = textColor;
    }
}
