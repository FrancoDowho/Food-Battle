using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

public class RouletteHUD : MonoBehaviour
{
    [SerializeField] GameObject _handsPrefab;
    [SerializeField] GameObject _textPrefab;
    bool _toTest;
    [SerializeField] float _testN;
    public void BuildRoulette(string[] OptionsNames)
    {
        float angleDistribution = 360 / OptionsNames.Length;
        float angle = 0;

        for (int i = 0; i < OptionsNames.Length; i++)
        {
            angle = i * angleDistribution;
            /*Vector3 position = new Vector3(
                Mathf.Cos(angle * Mathf.Deg2Rad) * _radius,
                Mathf.Sin(angle * Mathf.Deg2Rad) * _radius,
                0
            );*/

            //PASAR A ONLINE CON SPAWNED
            GameObject line = Instantiate(_handsPrefab, transform.position, Quaternion.identity);
            line.transform.SetParent(transform);
            float zRotation = angle + angleDistribution; 
            line.transform.rotation = Quaternion.Euler(0, 0, zRotation);
            line.GetComponent<RectTransform>().localScale = _handsPrefab.GetComponent<RectTransform>().localScale;

            GameObject txtOption = Instantiate(_textPrefab, transform.position, Quaternion.identity);
            txtOption.transform.SetParent(transform);
            txtOption.gameObject.GetComponentInChildren<TMP_Text>().text = OptionsNames[i];
            txtOption.transform.rotation = Quaternion.Euler(0, 0, zRotation - (angleDistribution / 2));
            txtOption.GetComponent<RectTransform>().localScale = _textPrefab.GetComponent<RectTransform>().localScale;        }

    }
    // Start is called before the first frame update
    void Start()
    {

        string[] h = new string[] { "1", "2","3","4","5","6","7"};
        BuildRoulette(h);

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
            _toTest = !_toTest;


        if (_toTest)
        {
            string[] h = new string[] { "1", "2", "3", "4", "5", "6", "7" };
            //float randomN = Random.Range(0, 360);
            //Debug.Log(_testN);
            transform.rotation = Quaternion.Euler(0, 0, -_testN);
            //Debug.Log(Roulette.choose(h.ToList(), randomN));
            Debug.Log(Roulette<string>.PickAWinner(h.ToList(), _testN));
        }
           
        
        
        
    }
}
