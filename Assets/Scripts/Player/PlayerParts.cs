using UnityEngine;

public class PlayerParts : MonoBehaviour
{
    private SpriteRenderer _weaponSlotL;

    void Start()
    {
        
    }
    void Update()
    {

    }

    public void UpgradeWeapon(Sprite newWeaponSprite)
    {
        // 1. 이펙트 생성 (파티클 등)
        // 2. 이미지 교체
        _weaponSlotL.sprite = newWeaponSprite;
        // 3. 연출 (크기 살짝 키우기)
        transform.localScale = Vector3.one * 1.2f; 
        // 4. 서서히 원래 크기로 (DOTween)
        
    }
}
