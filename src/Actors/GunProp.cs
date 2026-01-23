using NEP.MonoDirector.Data;

using Il2CppSLZ.Marrow;
using Il2CppSLZ.Marrow.Warehouse;
using Il2CppSLZ.Marrow.Data;
using Il2CppSLZ.Marrow.Combat;
using Il2CppSLZ.Marrow.Pool;

namespace NEP.MonoDirector.Actors
{
    [MelonLoader.RegisterTypeInIl2Cpp]
    public class GunProp(IntPtr ptr) : Prop(ptr)
    {
        public Gun Gun { get => gun; }

        private Gun gun;

        protected override void Awake()
        {
            base.Awake();

            propFrames = new List<ObjectFrame>();
            actionFrames = new List<ActionFrame>();
        }

        public override void OnSceneBegin()
        {
            base.OnSceneBegin();

            foreach(ActionFrame actionFrame in actionFrames)
            {
                actionFrame.Reset();
            }
        }

        public void GunFakeFire()
        {
            gun.cartridgeState = Gun.CartridgeStates.SPENT;
            gun.UpdateArt();
            
            MuzzleFlash();
            //EjectCasing();
            gun.gunSFX.GunShot();
        }

        public void SetGun(Gun gun)
        {
            this.gun = gun;
            SlideVirtualController slideVirtualController = gun.slideVirtualController;
            if (slideVirtualController != null)
            {
                gun.slideVirtualController.OnSlideGrabbed = gun.slideVirtualController.OnSlideGrabbed + new System.Action(() => RecordSlideGrabbed());
                gun.slideVirtualController.OnSlideReleased = gun.slideVirtualController.OnSlideReleased + new System.Action(() => RecordSlideReleased());
                gun.slideVirtualController.OnSlidePulled = gun.slideVirtualController.OnSlidePulled + new System.Action(() => RecordSlidePulled());
                gun.slideVirtualController.OnSlideUpdate = gun.slideVirtualController.OnSlideUpdate + new System.Action<float>((float perc) => RecordSlideUpdate(perc));
                gun.slideVirtualController.OnSlideReturned = gun.slideVirtualController.OnSlideReturned + new System.Action(() => RecordSlideReturned());
            }
            if(gun.internalMagazine != null)
            {
                _prevInternalMagazineAmmoCount = gun.MagazineState.AmmoCount;
                gun.MagazineState.onAmmoChange = gun.MagazineState.onAmmoChange + new System.Action<int>((int count) => OnAmmoChanged_InternalMagazine(count));
            }
        }

        private void MuzzleFlash()
        {
            Spawnable muzzleFlashSpawnable = new Spawnable()
            {
                crateRef = gun.muzzleFlareSpawnable.crateRef
            };
            AssetSpawner.Register(muzzleFlashSpawnable);

            AssetSpawner.Spawn(muzzleFlashSpawnable,
                gun.firePointTransform.position,
                gun.firePointTransform.rotation);
        }

        private void EjectCasing()
        {
            Spawnable cartridgeSpawnable = new Spawnable()
            {
                crateRef = gun.defaultCartridge.cartridgeCaseSpawnable.crateRef
            };
            AssetSpawner.Register(cartridgeSpawnable);

            AssetSpawner.Spawn(cartridgeSpawnable,
                gun.shellSpawnTransform.position,
                gun.shellOrientationTransform.rotation);
        }

        public void InsertMagState(CartridgeData cartridgeData, MagazineData magazineData, int count)
        {
            if (gun.internalMagazine != null)
            {
                gun.MagazineState.Initialize(cartridgeData, count);
            }
            else
            {
                MagazineState magazineState = new MagazineState()
                {
                    cartridgeData = cartridgeData,
                    magazineData = magazineData
                };
                magazineState.Initialize(cartridgeData, count);
                gun.MagazineState = magazineState;
            }

            // gun.UpdateMagazineArt();
        }

        public void AddMagState(CartridgeData cartridgeData, int amount)
        {
            gun.MagazineState.AddCartridge(amount, cartridgeData);
            // gun.UpdateMagazineArt();
        }

        private int _prevInternalMagazineAmmoCount;
        public void OnAmmoChanged_InternalMagazine(int count)
        {
            if (_prevInternalMagazineAmmoCount < count)
            {
                int amount = count - _prevInternalMagazineAmmoCount;
                _prevInternalMagazineAmmoCount = count;
                CartridgeData cartridgeData = gun.MagazineState.GetCartridgeData();
                RecordAction(new System.Action(() => AddMagState(cartridgeData, amount)));
            }
        }

        public void RemoveMagState()
        {
            gun.MagazineState = null;
        }

        public void RecordSlideGrabbed()
        {
            RecordAction(new System.Action(() => gun.slideVirtualController.OnSlideGrabbed.Invoke()));
        }
        public void RecordSlideReleased()
        {
            RecordAction(new System.Action(() => gun.slideVirtualController.OnSlideReleased.Invoke()));
        }
        public void RecordSlidePulled()
        {
            RecordAction(new System.Action(() => gun.slideVirtualController.OnSlidePulled.Invoke()));
        }
        public void RecordSlideUpdate(float perc)
        {
            RecordAction(new System.Action(() => gun.slideVirtualController.OnSlideUpdate.Invoke(perc)));
        }
        public void RecordSlideReturned()
        {
            RecordAction(new System.Action(() => gun.slideVirtualController.OnSlideReturned.Invoke()));
        }
    }
}
