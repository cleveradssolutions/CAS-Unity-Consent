using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace CAS.UserConsent
{
    [AddComponentMenu( "CleverAdsSolutions/UserConsent/Audience Definition UI" )]
    public sealed class AudienceDefinitionUI : MonoBehaviour
    {
        [Range( 0.05f, 1.0f )]
        [SerializeField]
        private float animationTime = 0.15f;
        [SerializeField]
        private int userInitialAge = 12;

        [SerializeField]
        private List<Text> components = new List<Text>();

        [Header( "Optional" )]
        public UnityEvent OnUnderAgeOfConsent;
        public UnityEvent OnConsentRequired;

        private Color selectedYearColor;
        private Color defaultYearColor;
        private Vector2[] textPositions;
        private int currentYear;
        private int selectedYear;
        private int swipeProcess;

        private void Start()
        {
            if (components.Count == 5)
            {
                defaultYearColor = components[1].color;
                selectedYearColor = components[2].color;
            }
            else
            {
                defaultYearColor = Color.clear;
                selectedYearColor = Color.clear;
            }
            currentYear = DateTime.Now.Year;
            textPositions = new Vector2[components.Count];
            selectedYear = currentYear - userInitialAge;
            for (int i = 0; i < components.Count; i++)
            {
                textPositions[i] = components[i].transform.position;
                components[i].text = ( selectedYear + 2 - i ).ToString();
            }

            SetTextColors();
        }

        public void OnBeginIncreasingYear()
        {
            swipeProcess = 1;
            IncreaseYear();
        }

        public void OnEndIncreasingYear()
        {
            swipeProcess = 0;
        }

        public void OnBeginDecreasingYear()
        {
            swipeProcess = -1;
            DecreaseYear();
        }

        public void OnEndDecreasingYear()
        {
            swipeProcess = 0;
        }

        public void ApplySelectedYear()
        {
            gameObject.SetActive( false );
            var audience = ConsentClient.SetYearOfBirth( selectedYear );
            if (audience == Audience.Children)
                OnUnderAgeOfConsent.Invoke();
            else
                OnConsentRequired.Invoke();
        }

        private void DecreaseYear()
        {
            StopAllCoroutines();
            for (int i = 1; i < components.Count; i++)
                StartCoroutine( MoveTexts( components[i].transform, textPositions[i - 1] ) );

            selectedYear--;
            var nextYear = selectedYear - 2;
            Text text = components[0];
            text.transform.position = textPositions[textPositions.Length - 1];
            text.text = nextYear.ToString();
            components.Remove( text );
            components.Add( text );

            SetTextColors();
        }

        private void IncreaseYear()
        {
            if (components[1].text == "")
                return;

            StopAllCoroutines();
            for (int i = 0; i < components.Count - 1; i++)
                StartCoroutine( MoveTexts( components[i].transform, textPositions[i + 1] ) );

            selectedYear++;
            var nextYear = selectedYear + 2;

            Text text = components[components.Count - 1];
            text.transform.position = textPositions[0];
            if (nextYear > currentYear)
                text.text = "";
            else
                text.text = nextYear.ToString();

            components.Remove( text );
            components.Insert( 0, text );

            SetTextColors();
        }

        private void SetTextColors()
        {
            if (selectedYearColor == Color.clear)
                return;
            for (int i = 0; i < components.Count; i++)
            {
                if (i == 2)
                    components[i].color = selectedYearColor;
                else
                    components[i].color = defaultYearColor;
            }
        }

        private IEnumerator MoveTexts( Transform moveText, Vector3 pos )
        {
            float multiplier = 1f / animationTime * ( pos - moveText.position ).magnitude;
            while (moveText.transform.position != pos)
            {
                moveText.position = Vector3.MoveTowards( moveText.position, pos, multiplier * Time.unscaledDeltaTime );
                yield return null;
            }
            if (swipeProcess < 0)
                DecreaseYear();
            else if (swipeProcess > 0)
                IncreaseYear();
        }
    }
}
