using UnityEngine;

public class SkillSelectPopup : PopupBase
{
    private float _moveSpeedTest = 2f;
    private float _beamSpeedTest = 15f;
    private float _beamWidthTest = 15f;

    public override void OnShow()
    {
        base.OnShow();
        Time.timeScale = 0f;    //게임 일시정지
    }
    public override void OnClose()
    {
        Time.timeScale = 1f;    //게임 재개
        base.OnClose();
    }

    public void OnClickUpgradeMoveSpeed()
    {
        GameManager.Instance.playerController.UpgradeMoveSpeed(_moveSpeedTest);
        OnClose();
    }
    public void OnClickUpgradeBeamSpeed()
    {
        GameManager.Instance.playerWeapon.UpgradeBeamSpeed(_beamSpeedTest);
        OnClose();
    }
    public void OnClickUpgradeBeamWidth()
    {
        GameManager.Instance.playerWeapon.UpgradeBeamWidth(_beamWidthTest);
        OnClose();
    }
}
