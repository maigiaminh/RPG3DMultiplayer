using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    [SerializeField] public Animator animator;
    #region Unarmed Hash
    private readonly int UnarmedFreeLook = Animator.StringToHash("Unarmed-FreeLook");
    private readonly int UnarmedTargeting = Animator.StringToHash("Unarmed-Targeting");
    private readonly int UnarmedDodge = Animator.StringToHash("Unarmed-Dodge");
    private readonly int UnarmedRoll = Animator.StringToHash("Unarmed-Roll");
    private readonly int UnarmedCrouch = Animator.StringToHash("Unarmed-Crouch");
    private readonly int UnarmedBlock = Animator.StringToHash("Unarmed-Block");
    private readonly int UnarmedTired = Animator.StringToHash("Unarmed-Tired");
    private readonly int UnarmedBlockGetHit = Animator.StringToHash("Unarmed-Block-GetHit");
    private readonly int UnarmedGetHit = Animator.StringToHash("Unarmed-GetHit");
    private readonly int UnarmedCastL = Animator.StringToHash("Unarmed-Cast-L");
    private readonly int UnarmedCastR = Animator.StringToHash("Unarmed-Cast-R");
    private readonly int UnarmedCastHead = Animator.StringToHash("Unarmed-Cast-Head");
    private readonly int UnarmedCastChest = Animator.StringToHash("Unarmed-Cast-Chest");
    private readonly int UnarmedCastUnderCenter = Animator.StringToHash("Unarmed-Cast-UnderCenter");
    private readonly int UnarmedCastForward = Animator.StringToHash("Unarmed-Cast-Forward");

    #endregion

    #region GreatSword Hash
    private readonly int GreatSwordFreeLook = Animator.StringToHash("GreatSword-FreeLook");
    private readonly int GreatSwordTargeting = Animator.StringToHash("GreatSword-Targeting");
    private readonly int GreatSwordDodge = Animator.StringToHash("GreatSword-Dodge");
    private readonly int GreatSwordRoll = Animator.StringToHash("GreatSword-Roll");
    private readonly int GreatSwordCrouch = Animator.StringToHash("GreatSword-Crouch");
    private readonly int GreatSwordBlock = Animator.StringToHash("GreatSword-Block");
    private readonly int GreatSwordSheath = Animator.StringToHash("GreatSword-Sheath");
    private readonly int GreatSwordUnSheath = Animator.StringToHash("GreatSword-Unsheath");
    private readonly int GreatSwordTired = Animator.StringToHash("GreatSword-Tired");
    private readonly int GreatSwordBlockGetHit = Animator.StringToHash("GreatSword-Block-GetHit");
    private readonly int GreatSwordGetHit = Animator.StringToHash("GreatSword-GetHit");
    private readonly int GreatSwordCastL = Animator.StringToHash("GreatSword-Cast-L");
    private readonly int GreatSwordCastR = Animator.StringToHash("GreatSword-Cast-R");
    private readonly int GreatSwordCastHead = Animator.StringToHash("GreatSword-Cast-Head");
    private readonly int GreatSwordCastChest = Animator.StringToHash("GreatSword-Cast-Chest");
    private readonly int GreatSwordCastUnderCenter = Animator.StringToHash("GreatSword-Cast-UnderCenter");
    private readonly int GreatSwordCastForward = Animator.StringToHash("GreatSword-Cast-Forward");
    private readonly int GreatSwordCastForward4cm = Animator.StringToHash("GreatSword-Cast-Forward4cm");
    private readonly int GreatSwordCastForward8cm = Animator.StringToHash("GreatSword-Cast-Forward8cm");
    #endregion

    #region Shield Hash
    private readonly int ShieldFreeLook = Animator.StringToHash("Shield-FreeLook");
    private readonly int ShieldTargeting = Animator.StringToHash("Shield-Targeting");
    private readonly int ShieldDodge = Animator.StringToHash("Shield-Dodge");
    private readonly int ShieldRoll = Animator.StringToHash("Shield-Roll");
    private readonly int ShieldCrouch = Animator.StringToHash("Shield-Crouch");
    private readonly int ShieldBlock = Animator.StringToHash("Shield-Block");
    private readonly int ShieldSheath = Animator.StringToHash("Shield-Sheath");
    private readonly int ShieldUnSheath = Animator.StringToHash("Shield-Unsheath");
    private readonly int ShieldTired = Animator.StringToHash("Shield-Tired");
    private readonly int ShieldBlockGetHit = Animator.StringToHash("Shield-Block-GetHit");
    private readonly int ShieldGetHit = Animator.StringToHash("Shield-GetHit");
    private readonly int ShieldCastL = Animator.StringToHash("Shield-Cast-L");
    private readonly int ShieldCastR = Animator.StringToHash("Shield-Cast-R");
    private readonly int ShieldCastHead = Animator.StringToHash("Shield-Cast-Head");
    private readonly int ShieldCastChest = Animator.StringToHash("Shield-Cast-Chest");
    private readonly int ShieldCastUnderCenter = Animator.StringToHash("Shield-Cast-UnderCenter");
    private readonly int ShieldCastForward = Animator.StringToHash("Shield-Cast-Forward");
    private readonly int ShieldCastForward4cm = Animator.StringToHash("Shield-Cast-Forward4cm");
    private readonly int ShieldCastForward8cm = Animator.StringToHash("Shield-Cast-Forward8cm");

    #endregion

    #region Bow Hash
    private readonly int BowFreeLook = Animator.StringToHash("Bow-FreeLook");
    private readonly int BowTargeting = Animator.StringToHash("Bow-Targeting");
    private readonly int BowDodge = Animator.StringToHash("Bow-Dodge");
    private readonly int BowRoll = Animator.StringToHash("Bow-Roll");
    private readonly int BowCrouch = Animator.StringToHash("Bow-Crouch");
    private readonly int BowBlock = Animator.StringToHash("Bow-Block");
    private readonly int BowSheath = Animator.StringToHash("Bow-Sheath");
    private readonly int BowUnSheath = Animator.StringToHash("Bow-Unsheath");
    private readonly int BowTired = Animator.StringToHash("Bow-Tired");
    private readonly int BowAiming = Animator.StringToHash("Bow-Aiming");
    private readonly int BowGetHit = Animator.StringToHash("Bow-GetHit");
    private readonly int BowCastL = Animator.StringToHash("Bow-Cast-L");
    private readonly int BowCastR = Animator.StringToHash("Bow-Cast-R");
    private readonly int BowCastHead = Animator.StringToHash("Bow-Cast-Head");
    private readonly int BowCastChest = Animator.StringToHash("Bow-Cast-Chest");
    private readonly int BowCastUnderCenter = Animator.StringToHash("Bow-Cast-UnderCenter");
    private readonly int BowCastForward = Animator.StringToHash("Bow-Cast-Forward");

    #endregion

    #region Dual Knife Hash
    private readonly int DualKnifeFreeLook = Animator.StringToHash("Knife-FreeLook");
    private readonly int DualKnifeTargeting = Animator.StringToHash("Knife-Targeting");
    private readonly int DualKnifeDodge = Animator.StringToHash("Knife-Dodge");
    private readonly int DualKnifeRoll = Animator.StringToHash("Knife-Roll");
    private readonly int DualKnifeCrouch = Animator.StringToHash("Knife-Crouch");
    private readonly int DualKnifeBlock = Animator.StringToHash("Knife-Block");
    private readonly int DualKnifeSheath = Animator.StringToHash("Knife-Sheath");
    private readonly int DualKnifeUnSheath = Animator.StringToHash("Knife-Unsheath");
    private readonly int DualKnifeTired = Animator.StringToHash("Knife-Tired");
    private readonly int DualKnifeBlockGetHit = Animator.StringToHash("Knife-Block-GetHit");
    private readonly int DualKnifeGetHit = Animator.StringToHash("Knife-GetHit");
    private readonly int DualKnifeCastL = Animator.StringToHash("Knife-Cast-L");
    private readonly int DualKnifeCastR = Animator.StringToHash("Knife-Cast-R");
    private readonly int DualKnifeCastHead = Animator.StringToHash("Knife-Cast-Head");
    private readonly int DualKnifeCastChest = Animator.StringToHash("Knife-Cast-Chest");
    private readonly int DualKnifeCastUnderCenter = Animator.StringToHash("Knife-Cast-UnderCenter");
    private readonly int DualKnifeCastForward = Animator.StringToHash("Knife-Cast-Forward");
    private readonly int DualKnifeCastForward4cm = Animator.StringToHash("Knife-Cast-Forward4cm");
    private readonly int DualKnifeCastForward8cm = Animator.StringToHash("Knife-Cast-Forward8cm");
    #endregion

    #region WizardStaff Hash
    private readonly int WizardStaffFreeLook = Animator.StringToHash("WizardStaff-FreeLook");
    private readonly int WizardStaffTargeting = Animator.StringToHash("WizardStaff-Targeting");
    private readonly int WizardStaffDodge = Animator.StringToHash("WizardStaff-Dodge");
    private readonly int WizardStaffRoll = Animator.StringToHash("WizardStaff-Roll");
    private readonly int WizardStaffCrouch = Animator.StringToHash("WizardStaff-Crouch");
    private readonly int WizardStaffBlock = Animator.StringToHash("WizardStaff-Block");
    private readonly int WizardStaffSheath = Animator.StringToHash("WizardStaff-Sheath");
    private readonly int WizardStaffUnSheath = Animator.StringToHash("WizardStaff-Unsheath");
    private readonly int WizardStaffTired = Animator.StringToHash("WizardStaff-Tired");
    private readonly int WizardStaffBlockGetHit = Animator.StringToHash("WizardStaff-Block-GetHit");
    private readonly int WizardStaffGetHit = Animator.StringToHash("WizardStaff-GetHit");
    private readonly int WizardStaffCastL = Animator.StringToHash("WizardStaff-Cast-L");
    private readonly int WizardStaffCastR = Animator.StringToHash("WizardStaff-Cast-R");
    private readonly int WizardStaffCastHead = Animator.StringToHash("WizardStaff-Cast-Head");
    private readonly int WizardStaffCastChest = Animator.StringToHash("WizardStaff-Cast-Chest");
    private readonly int WizardStaffCastUnderCenter = Animator.StringToHash("WizardStaff-Cast-UnderCenter");
    private readonly int WizardStaffCastForward = Animator.StringToHash("WizardStaff-Cast-Forward");
    private readonly int WizardStaffCastForward4cm = Animator.StringToHash("WizardStaff-Cast-Forward4cm");
    private readonly int WizardStaffCastForward8cm = Animator.StringToHash("WizardStaff-Cast-Forward8cm");
    #endregion

    #region Formal Hash
    private readonly int FormalFreeLook = Animator.StringToHash("Formal-FreeLook");
    private readonly int FormalSheathGreatSword = Animator.StringToHash("Formal-Sheath-GreatSword");
    private readonly int FormalSheathUnarmed = Animator.StringToHash("Formal-Sheath-Unarmed");
    private readonly int FormalSheathShield = Animator.StringToHash("Formal-Sheath-Shield");
    private readonly int FormalSheathBow = Animator.StringToHash("Formal-Sheath-Bow");
    private readonly int FormalSheathDualKnife = Animator.StringToHash("Formal-Sheath-Knife");
    private readonly int FormalSheathWizardStaff = Animator.StringToHash("Formal-Sheath-WizardStaff");

    #endregion

    private Dictionary<(int weapon, int position), int> castHashDict = new Dictionary<(int weapon, int position), int>();
    public PlayerAnimationController()
    {
        castHashDict = new Dictionary<(int weapon, int position), int>
        {
            { (0, 0), UnarmedCastL },
            { (0, 1), UnarmedCastR },
            { (0, 2), UnarmedCastHead },
            { (0, 3), UnarmedCastChest },
            { (0, 6), UnarmedCastUnderCenter },
            { (0, 7), UnarmedCastForward },
            { (0, 8), UnarmedCastForward },
            { (0, 9), UnarmedCastForward },
            { (0, 10), UnarmedCastForward },

            { (1, 0), GreatSwordCastL },
            { (1, 1), GreatSwordCastR },
            { (1, 2), GreatSwordCastHead },
            { (1, 3), GreatSwordCastChest },
            { (1, 6), GreatSwordCastUnderCenter },
            { (1, 7), GreatSwordCastForward },
            { (1, 8), GreatSwordCastForward },
            { (1, 9), GreatSwordCastForward8cm },
            { (1, 10), GreatSwordCastForward4cm },

            { (2, 0), BowCastL },
            { (2, 1), BowCastR },
            { (2, 2), BowCastHead },
            { (2, 3), BowCastChest },
            { (2, 6), BowCastUnderCenter },
            { (2, 7), BowCastForward },
            { (2, 8), BowCastForward },
            { (2, 9), BowCastForward },
            { (2, 10), BowCastForward },

            { (3, 0), ShieldCastL },
            { (3, 1), ShieldCastR },
            { (3, 2), ShieldCastHead },
            { (3, 3), ShieldCastChest },
            { (3, 6), ShieldCastUnderCenter },
            { (3, 7), ShieldCastForward },
            { (3, 8), ShieldCastForward },
            { (3, 9), ShieldCastForward8cm },
            { (3, 10), ShieldCastForward4cm },

            { (4, 0), DualKnifeCastL },
            { (4, 1), DualKnifeCastR },
            { (4, 2), DualKnifeCastHead },
            { (4, 3), DualKnifeCastChest },
            { (4, 6), DualKnifeCastUnderCenter },
            { (4, 7), DualKnifeCastForward },
            { (4, 8), DualKnifeCastForward },
            { (4, 9), DualKnifeCastForward8cm },
            { (4, 10), DualKnifeCastForward4cm },

            { (5, 0), WizardStaffCastL },
            { (5, 1), WizardStaffCastR },
            { (5, 2), WizardStaffCastHead },
            { (5, 3), WizardStaffCastChest },
            { (5, 6), WizardStaffCastUnderCenter },
            { (5, 7), WizardStaffCastForward },
            { (5, 8), WizardStaffCastForward },
            { (5, 9), WizardStaffCastForward8cm },
            { (5, 10), WizardStaffCastForward4cm },
        };
    }

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public int GetFreeLookBlendTree(int currentWeapon)
    {
        if (currentWeapon == 0) return UnarmedFreeLook;
        if (currentWeapon == 1) return GreatSwordFreeLook;
        if (currentWeapon == 2) return BowFreeLook;
        if (currentWeapon == 3) return ShieldFreeLook;
        if(currentWeapon == 4) return DualKnifeFreeLook;
        if(currentWeapon == 5) return WizardStaffFreeLook;

        if (currentWeapon == -1) return FormalFreeLook;
        
        return 0;
    }

    public int GetTargetingBlendTree(int currentWeapon)
    {
        if (currentWeapon == 0) return UnarmedTargeting;
        if (currentWeapon == 1) return GreatSwordTargeting;
        if (currentWeapon == 2) return BowTargeting;
        if (currentWeapon == 3) return ShieldTargeting;
        if (currentWeapon == 4) return DualKnifeTargeting;
        if(currentWeapon == 5) return WizardStaffTargeting;

        return 0;
    }

    public int GetDodgeBlendTree(int currentWeapon)
    {
        if (currentWeapon == 0) return UnarmedDodge;
        if (currentWeapon == 1) return GreatSwordDodge;
        if (currentWeapon == 2) return BowDodge;
        if (currentWeapon == 3) return ShieldDodge;
        if (currentWeapon == 4) return DualKnifeDodge;
        if(currentWeapon == 5) return WizardStaffDodge;

        return 0;
    }

    public int GetRollBlendTree(int currentWeapon)
    {
        if (currentWeapon == 0) return UnarmedRoll;
        if (currentWeapon == 1) return GreatSwordRoll;
        if (currentWeapon == 2) return BowRoll;
        if (currentWeapon == 3) return ShieldRoll;
        if (currentWeapon == 4) return DualKnifeRoll;
        if(currentWeapon == 5) return WizardStaffRoll;

        return 0;
    }

    public int GetBlock(int currentWeapon)
    {
        if (currentWeapon == 0) return UnarmedBlock;
        if (currentWeapon == 1) return GreatSwordBlock;
        if (currentWeapon == 2) return BowBlock;
        if (currentWeapon == 3) return ShieldBlock;
        if (currentWeapon == 4) return DualKnifeBlock;
        if(currentWeapon == 5) return WizardStaffBlock;

        return 0;
    }

    public int GetCrouch(int currentWeapon)
    {
        if (currentWeapon == 0) return UnarmedCrouch;
        if (currentWeapon == 1) return GreatSwordCrouch;
        if (currentWeapon == 2) return BowCrouch;
        if (currentWeapon == 3) return ShieldCrouch;
        if (currentWeapon == 4) return DualKnifeCrouch;
        if(currentWeapon == 5) return WizardStaffCrouch;

        return 0;
    }

    public int GetTired(int currentWeapon)
    {
        if (currentWeapon == 0) return UnarmedTired;
        if (currentWeapon == 1) return GreatSwordTired;
        if (currentWeapon == 2) return BowTired;
        if (currentWeapon == 3) return ShieldTired;
        if (currentWeapon == 4) return DualKnifeTired;
        if(currentWeapon == 5) return WizardStaffTired;

        return 0;
    }

    public int GetSheath(int currentWeapon)
    {
        if (currentWeapon == 1) return GreatSwordSheath;
        if (currentWeapon == 2) return BowSheath;
        if (currentWeapon == 3) return ShieldSheath;
        if (currentWeapon == 4) return DualKnifeSheath;
        if(currentWeapon == 5) return WizardStaffSheath;

        return 0;
    }

    public int GetUnSheath(int currentWeapon)
    {
        if (currentWeapon == 1) return GreatSwordUnSheath;
        if (currentWeapon == 2) return BowUnSheath;
        if (currentWeapon == 3) return ShieldUnSheath;
        if (currentWeapon == 4) return DualKnifeUnSheath;
        if(currentWeapon == 5) return WizardStaffUnSheath;

        return 0;
    }

    public int GetFormalStand(int currentWeapon)
    {
        if (currentWeapon == 0) return FormalSheathUnarmed;
        if (currentWeapon == 1) return FormalSheathGreatSword;
        if (currentWeapon == 2) return FormalSheathBow;
        if (currentWeapon == 3) return FormalSheathShield;
        if (currentWeapon == 4) return FormalSheathDualKnife;
        if(currentWeapon == 5) return FormalSheathWizardStaff;

        return 0;
    }

    public int GetAiming(int currentWeapon)
    {
        if (currentWeapon == 2) return BowAiming;

        return 0;
    }

    public int GetBlockHit(int currentWeapon){
        if (currentWeapon == 0) return UnarmedBlockGetHit;
        if (currentWeapon == 1) return GreatSwordBlockGetHit;
        if (currentWeapon == 2) return 0;
        if (currentWeapon == 3) return ShieldBlockGetHit;
        if (currentWeapon == 4) return DualKnifeBlockGetHit;
        if(currentWeapon == 5) return WizardStaffBlockGetHit;

        return 0;
    }

    public int GetHit(int currentWeapon){
        if (currentWeapon == 0) return UnarmedGetHit;
        if (currentWeapon == 1) return GreatSwordGetHit;
        if (currentWeapon == 2) return BowGetHit;
        if (currentWeapon == 3) return ShieldGetHit;
        if (currentWeapon == 4) return DualKnifeGetHit;
        if(currentWeapon == 5) return WizardStaffGetHit;

        return 0;
    }

    public int GetCastHash(int currentWeapon, int position)
    {
        if (castHashDict.TryGetValue((currentWeapon, position), out int hash)){
            return hash;
        }

        return 0;
    }


    public void GetWeapon(int currentWeapon)
    {
        animator.SetInteger("Weapon", currentWeapon);
    }

    public void EnableRunningAttack()
    {
        animator.SetBool("Running-Attack", true);
    }

    public void DisbleRunningAttack()
    {
        animator.SetBool("Running-Attack", false);
    }

    public void SetCastingSkillPosition(int pos)
    {
        animator.SetInteger("Position", pos);
    }

    public void EnableRunningCastSkill()
    {
        animator.SetBool("Running-Cast-Skill", true);
    }

    public void DisableRunningCastSkill()
    {
        animator.SetBool("Running-Cast-Skill", false);
    }
    public void DoneAnimation()
    {
        animator.SetBool("Done-Anim", true);
    }

    public void StartAnimation()
    {
        animator.SetBool("Done-Anim", false);
    }

    public void StartAiming(){
        animator.SetBool("IsAiming", true);
    }

    public void DoneAiming(){
        animator.SetBool("IsAiming", false);
    }
}
