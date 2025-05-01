    using UnityEngine;

    [System.Serializable]
    public class EnemyType
    {
        public string name;         
        public GameObject enemyPrefab; 
        public float speed;         
        public int maxHP;           
        public int damage;         
        public int coinReward;     
        public string identifier;   
    }
