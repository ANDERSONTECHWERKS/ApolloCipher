/*
 *  OVERLORD SHIP AI
 *  
 *  Overlord Ship AI is a generic ship response system intended to provide automated response in accordance with a ship's alert conditions.
 *  
 *  Features:
 *   - Alert Conditions to set ship conditions based off ship posture
 *   - Automatic response to shield damage and shield heat
 *   - Custom Cockpit HUD Elements
 *   - Color and Behavior customization options
 *  
 *   Instructions:
 *   - Install this script in any programmable block.
 *   - In a button panel / terminal / cockpit, drag and drop the programmable block you installed the script into to your toolbar,
 *     and when prompted for an argument use all the following to set your ship conditions:
 *  	- mooredAlert
 *  	- standardAlert
 *  	- yellowAlert
 *  	- redAlert
 *  
 *   - The grid will automatically go to red alert when it detects shield damage and heat at the same time. 
 *   - You will want to explicitly go into yellow alert before battle for best results.
 */

// USER INSTRUCTIONS:

// If you want to use the aimbot, do the following:
// - Put the gyros you want to aim with into a group named "[OL]AimGyros", and the fixed weapons into a group called "[OL]AimGuns".
// - If you are using a railgun: the name of your Railgun or Coilgun MUST have the world 'railgun' or 'coilgun' in the blockname.
// - In your toolbar: Drag-and-Drop the programmable block you put this script into to your toolbar, then use the "Run" command with the argument: "toggle".
// --> "Fire" may also be used, and will auto-fire when within deviation, but manually fire your weapons if you wanna be certain.

// If you want to use the railgundoors feature, do the following:
// - Put the doors covering your railguns into a group named "OLGunDoors"
// - The system should automatically open/close the doors to your railgun

// To manually set alert conditions, do the following:
// - Drag and drop this programmable block into your toolbar with the following alert conditions:
// -> mooredAlert
// -> standardAlert
// -> yellowAlert
// -> redAlert

//RAILGUN DOORS
// To set a group of doors to open/close when a designated weapon is 'ready' (Currently specified by me)
// Do the following:
// - Set all relavant doors into a group named [OL]GunDoors
// - Add the word 'railgun' into the name of the the gun you want to be monitored for readiness status

/*
        RELEVANT GROUPNAMES / DEFAULTS (IF BEHAVIOR DESIRED):
        [OL]IntAlertLights
        [OL]IntRotLights
        [OL]Spotlights
        [OL]DCWelders
        [OL]PMWWelders
        [OL]GunDoors --> These doors will open/close depending on whether or not they detect a specified railgun is ready to fire.
        [OL]IntSecSensors
        [OL]StatusLCD
        */
// OVERLORD Script Customization:

// Debug and logging
bool debug = true;

// Runtime frequency
UpdateFrequency updateFrequency = UpdateFrequency.Update10;

// Lighting Options

// InteriorLightsMatchAlertLightColor: In this mode - the 'alert lights' will be every light accessible on the ship.
// If set to false: This mode will only use rotatinglights to indicate what 'level' of alert you are at.
private bool InteriorLightsIndicateAlertLevel = true;

// Choose the colors of your thrusters during operation (moored will always be GHOSTWHITE)
private Color hydroThrusterColorIdle = ColorExtensions.HexToColor("333333");
private Color hydroThrusterColorFull = ColorExtensions.HexToColor("FF7700");
private Color ionThrusterColorIdle = ColorExtensions.HexToColor("333333");
private Color ionThrusterColorFull = ColorExtensions.HexToColor("FF7700");

// Choose the colors of your ship's lights for each alert condition
private Color MooredAlertLightColor = ColorExtensions.HexToColor("FFFFFF");
private Color StandardAlertLightColor = ColorExtensions.HexToColor("174973");
private Color YellowAlertLightColor = ColorExtensions.HexToColor("E8B323");
private Color RedAlertLightColor = ColorExtensions.HexToColor("B20909");
private Color BlueAlertLightColor = ColorExtensions.HexToColor("008CFF");
private Color DefaultInteriorLightsColor = ColorExtensions.HexToColor("FF7700");

//Choose colors for reflector lights (spotlights, rotating lights)
private Color SpotLightColor = ColorExtensions.HexToColor("FFFFFF");

// Do you want beacons / antennae active while in standard cruising?
// All beacons/anennae currently turn OFF while in battle.
private bool AntennaeActiveYellowAlert = true;
private bool BeaconActiveYellowAlertAndAbove = true;

// Shield percentage before the ship goes to red alert
int RedAlertDamageThreshold = 60;

// Server speed limit? Default: SW5.0 Speedlimit
float ServerSpeedLimit = 350f;

// Choose whether or not hydrogen thrusters are turned on during mooredAlert and standardAlert
bool HydrogenThrustONmooredAlert = false;
bool HydrogenThrustONstandardCruising = false;

string GunGroupName = "AimGuns";
string GyroGroupName = "AimGyros";
string RailGunDoorsName = "GunDoors";

Action Ƀ=null;Ȱ Ʉ=Ȱ.Ȳ;Ȱ Ɂ=Ȱ.Ȳ;float ȧ=0f;bool ȿ=false;bool Ȩ=false;int ȩ=0;string Ȫ="NOTHREATDETECTED";string ȫ=
"NOTHREATREL";string Ȭ="NOTHREATPOS";string ȭ="NOTHREATVELOCITY";StringBuilder Ȯ=new StringBuilder();IMyTextSurface ȯ;private enum Ȱ{
ȱ,Ȳ,ȳ,ȴ,ȵ,ȶ,ȷ,ȸ}ǂ ȹ;static Ç Ⱥ;static s Ȼ;static string ȼ="[OL]";string Ƚ="BEGIN LOG";int Ⱦ=0;double ɂ=0.5;double ɀ=0;
MyCommandLine Ʌ=new MyCommandLine();Vector3 ɢ;IMyBlockGroup ɣ;IMyBlockGroup ɤ;IMyBlockGroup ɥ;IMyBlockGroup ɦ;IMyBlockGroup ɧ;
IMyBlockGroup ɨ;IMyBlockGroup ɩ;IMyBlockGroup ɪ;IMyBlockGroup ɫ;IMyBlockGroup ɬ;List<MyDefinitionId>ɭ=new List<MyDefinitionId>();List
<IMyEntity>ɯ=new List<IMyEntity>();List<IMyTimerBlock>ɻ=new List<IMyTimerBlock>();List<IMyTextPanel>ɰ=new List<
IMyTextPanel>();List<IMyBatteryBlock>ɱ=new List<IMyBatteryBlock>();List<IMyGasTank>ɲ=new List<IMyGasTank>();List<IMyLightingBlock>ɳ=
new List<IMyLightingBlock>();List<IMyDoor>ɴ=new List<IMyDoor>();List<IMyTextSurfaceProvider>ɵ=new List<
IMyTextSurfaceProvider>();List<IMySensorBlock>ɶ=new List<IMySensorBlock>();List<IMyJumpDrive>ɷ=new List<IMyJumpDrive>();List<IMyReactor>ɸ=new
List<IMyReactor>();List<IMyCockpit>ɹ=new List<IMyCockpit>();List<IMyMedicalRoom>ɺ=new List<IMyMedicalRoom>();List<
IMyRadioAntenna>ɼ=new List<IMyRadioAntenna>();List<IMyBeacon>ɮ=new List<IMyBeacon>();List<IMyLandingGear>ɡ=new List<IMyLandingGear>();
List<IMyGravityGenerator>ɒ=new List<IMyGravityGenerator>();List<IMySensorBlock>ɇ=new List<IMySensorBlock>();List<
IMyShipConnector>Ɉ=new List<IMyShipConnector>();List<IMyButtonPanel>ɉ=new List<IMyButtonPanel>();List<IMyCameraBlock>Ɋ=new List<
IMyCameraBlock>();List<IMyGyro>ɋ=new List<IMyGyro>();List<IMyRefinery>Ɍ=new List<IMyRefinery>();List<IMyProgrammableBlock>ɍ=new List<
IMyProgrammableBlock>();List<IMyAirtightHangarDoor>Ɏ=new List<IMyAirtightHangarDoor>();List<IMyShipWelder>ɏ=new List<IMyShipWelder>();List<
IMySoundBlock>ɐ=new List<IMySoundBlock>();List<IMyLargeInteriorTurret>ɑ=new List<IMyLargeInteriorTurret>();List<IMyWarhead>ɓ=new List
<IMyWarhead>();List<IMyMotorAdvancedStator>ɟ=new List<IMyMotorAdvancedStator>();List<IMyTextSurfaceProvider>ɔ=new List<
IMyTextSurfaceProvider>();List<IMyTerminalBlock>ɕ=new List<IMyTerminalBlock>();List<IMyTerminalBlock>ɖ=new List<IMyTerminalBlock>();List<
IMyTerminalBlock>ɗ=new List<IMyTerminalBlock>();List<IMyDoor>ɘ=new List<IMyDoor>();List<IMyThrust>ə=new List<IMyThrust>();List<IMyThrust
>ɚ=new List<IMyThrust>();List<IMyThrust>ɛ=new List<IMyThrust>();List<IMyRemoteControl>ɜ=new List<IMyRemoteControl>();List
<IMyEventControllerBlock>ɝ=new List<IMyEventControllerBlock>();List<IMyOffensiveCombatBlock>ɞ=new List<
IMyOffensiveCombatBlock>();List<IMyFlightMovementBlock>ɠ=new List<IMyFlightMovementBlock>();List<IMyDefensiveCombatBlock>Ɇ=new List<
IMyDefensiveCombatBlock>();List<IMyPathRecorderBlock>Ȧ=new List<IMyPathRecorderBlock>();List<IMyShipController>ȋ=new List<IMyShipController>();
Dictionary<string,Action>ǰ=new Dictionary<string,Action>(StringComparer.OrdinalIgnoreCase);Dictionary<string,Action>Ǳ=new
Dictionary<string,Action>(StringComparer.OrdinalIgnoreCase);Dictionary<MyDetectedEntityInfo,float>ǲ=new Dictionary<
MyDetectedEntityInfo,float>();RectangleF ǳ=new RectangleF();MySpriteDrawFrame Ǵ=new MySpriteDrawFrame();Vector2 ǵ=new Vector2(0,0);Vector2 Ƕ
=new Vector2(0,-60);Vector2 Ƿ=new Vector2(0,100);Vector2 Ǹ=new Vector2(0,-50);Vector2 ǹ=new Vector2(0,10);Vector2 Ǻ=new
Vector2(0,30);MySprite Ǽ;int Ȉ=0;IMyTextSurface ǽ=null;IMyTextSurface Ǿ=null;IMyTextSurface ǿ=null;IMyTextSurface Ȁ=null;
IMyTextSurface ȁ=null;IMyTextSurface Ȃ=null;List<Vector3>ȃ=new List<Vector3>{Vector3.Zero,Vector3.Zero,Vector3.Zero};
MyDetectedEntityInfo Ȅ;Queue<string>ȅ;string Ȇ;string[]ȇ;string ȉ="OLBegin=true;";bool Ǯ=false;bool ǔ=false;Dictionary<string,string>Ǭ;
string[]Ǖ;string ǖ;string Ǘ;string ǘ;string Ǚ;string ǚ;MyDetectedEntityInfo Ǜ;HashSet<MyDetectedEntityInfo>ǜ=new HashSet<
MyDetectedEntityInfo>();MySprite ǝ;MySprite Ǟ;MySprite ǟ;MySprite Ǡ;MySprite ǡ;MySprite Ǣ;MySprite ǣ;MySprite Ǥ;MySprite ǥ;MySprite Ǧ;
MySprite ǧ;MySprite Ǩ;MySprite ǩ;MySprite Ǫ;MySprite ǫ;MySprite ǻ;MySprite ǭ;MySprite Ȋ;MySprite Ȗ;IMyJumpDrive ȗ;IMyJumpDrive Ș
;IMyBeacon ș;string Ț(Ȱ ț){switch(ț){case Ȱ.ȷ:return"Blue Alert";case Ȱ.ȱ:return"<- MOORED ->";case Ȱ.ȸ:return
"S T E A L T H";case Ȱ.Ȳ:return"Standard Alert";case Ȱ.ȳ:return"Yellow Alert";case Ȱ.ȴ:return"RED ALERT";default:return
"Unknown Alert status!";}}IEnumerable<VRageMath.Vector3D>Ȝ(){yield return Vector3.Zero;}IEnumerator ȝ(bool Ȟ){while(true){if(Ȟ){Ⱦ++;ȅ.Enqueue(
$"DEBUG: Standard Alert CheckIter Count:  {Ⱦ} times!");}ˁ(Ȼ);ʴ(ɷ);ʱ(ȋ);ʅ(ɔ);if(ɖ.Count>0){IMyTerminalBlock ȟ=ɖ.Find(Ƞ=>Ƞ.CustomName.ToLower().Contains("railgun")||Ƞ.
CustomName.ToLower().Contains("coilgun"));ʁ(ȟ,ɘ);}ʼ(ɹ);yield return true;}}IEnumerator ȡ(bool Ȟ){while(true){if(Ȟ){Ⱦ++;ȅ.Enqueue(
$"Yellow Alert CheckIter Count:  {Ⱦ} times!");}ˁ(Ȼ);ʴ(ɷ);ʱ(ȋ);ʾ(ɟ);if(ɖ.Count>0){IMyTerminalBlock ȟ=ɖ.Find(Ƞ=>Ƞ.CustomName.ToLower().Contains("railgun")||Ƞ.
CustomName.ToLower().Contains("coilgun"));ʁ(ȟ,ɘ);}ʼ(ɹ);yield return true;}}IEnumerator Ȣ(bool Ȟ){while(true){if(Ȟ){Ⱦ++;ȅ.Enqueue(
$"Red Alert CheckIter Count:  {Ⱦ} times!");}ˁ(Ȼ);ʴ(ɷ);ʱ(ȋ);ʾ(ɟ);if(ɖ.Count>0){IMyTerminalBlock ȟ=ɖ.Find(Ƞ=>Ƞ.CustomName.ToLower().Contains("railgun")||Ƞ.
CustomName.ToLower().Contains("coilgun"));ʁ(ȟ,ɘ);}ʼ(ɹ);yield return true;}}IEnumerator ȣ(bool Ȟ){while(true){if(Ȟ){Ⱦ++;ȅ.Enqueue(
$"Blue Alert CheckIter Count: {Ⱦ} times!");}ˁ(Ȼ);ʴ(ɷ);ʱ(ȋ);ʼ(ɹ);yield return true;}}IEnumerator Ȥ(bool Ȟ){while(true){if(Ȟ){Ⱦ++;ȅ.Enqueue(
$"Moored Alert Checked {Ⱦ} times!");}ˁ(Ȼ);ʴ(ɷ);ʱ(ȋ);ʼ(ɹ);yield return true;}}void ȥ(){Ɂ=Ʉ;Ʉ=Ȱ.ȱ;foreach(IMyAirtightHangarDoor Ȕ in Ɏ){if(
GridTerminalSystem.CanAccess(Ȕ)){Ȕ.ShowInTerminal=false;Ȕ.ShowOnHUD=false;if(Ȕ.Status==DoorStatus.Closed){Ȕ.OpenDoor();}else{}}}foreach(
IMyEventControllerBlock m in ɝ){if(GridTerminalSystem.CanAccess(m)){m.Enabled=false;}}foreach(IMyOffensiveCombatBlock m in ɞ){if(
GridTerminalSystem.CanAccess(m)){m.Enabled=false;m.UpdateTargetInterval=10;m.TargetPriority=OffensiveCombatTargetPriority.Largest;}}
foreach(IMyFlightMovementBlock m in ɠ){if(GridTerminalSystem.CanAccess(m)){m.Enabled|=false;m.AlignToPGravity=false;m.
CollisionAvoidance=false;m.FlightMode=FlightMode.OneWay;m.MinimalAltitude=100000f;m.PrecisionMode=false;m.SpeedLimit=350f;}}foreach(
IMyDefensiveCombatBlock m in Ɇ){if(GridTerminalSystem.CanAccess(m)){m.Enabled=false;m.FleeTrigger=FleeTrigger.WhenTakingDamage;m.ApplyAction(
"SetAttackMode_EnemiesOnly");m.ApplyAction("SetTargetingGroup_Weapons");}}foreach(IMyPathRecorderBlock m in Ȧ){if(GridTerminalSystem.CanAccess(m)){
m.Enabled=false;}}foreach(IMyRemoteControl Ȍ in ɜ){if(GridTerminalSystem.CanAccess(Ȍ)){Ȍ.DampenersOverride=false;Ȍ.
IsMainCockpit=false;}if(Ȍ.CanControlShip){}}foreach(IMyThrust ȍ in ɚ){if(GridTerminalSystem.CanAccess(ȍ)){ȍ.Enabled=
HydrogenThrustONmooredAlert;}}foreach(IMyThrust Ȏ in ɛ){if(GridTerminalSystem.CanAccess(Ȏ)){Ȏ.Enabled=true;}}foreach(IMyLightingBlock ȏ in ɳ){if(
GridTerminalSystem.CanAccess(ȏ)){if(ȏ.BlockDefinition.SubtypeName.Equals("RotatingLightLarge")){ȏ.Enabled=false;ȏ.Color=
MooredAlertLightColor;ȏ.BlinkIntervalSeconds=0;ȏ.BlinkLength=0f;ȏ.BlinkOffset=0f;ȏ.Falloff=5f;ȏ.Intensity=5.0f;ȏ.Radius=160;}else if(ȏ.
BlockDefinition.SubtypeName.Equals("LargeBlockFrontLight")){ȏ.Enabled=false;ȏ.Color=SpotLightColor;ȏ.BlinkIntervalSeconds=0;ȏ.
BlinkLength=0f;ȏ.BlinkOffset=0f;ȏ.Falloff=100f;ȏ.Intensity=5.0f;ȏ.Radius=160f;}else if(ȏ is IMyInteriorLight&&
InteriorLightsIndicateAlertLevel){ȏ.Enabled=true;ȏ.Color=MooredAlertLightColor;ȏ.BlinkIntervalSeconds=0f;ȏ.BlinkLength=0f;ȏ.BlinkOffset=0f;ȏ.Falloff=5f;
ȏ.Intensity=5.0f;ȏ.Radius=160f;}else{ȏ.Enabled=true;ȏ.Color=DefaultInteriorLightsColor;ȏ.BlinkIntervalSeconds=0f;ȏ.
BlinkLength=0f;ȏ.BlinkOffset=0f;ȏ.Falloff=5f;ȏ.Intensity=5.0f;ȏ.Radius=160f;}}}foreach(IMyDoor ǯ in ɴ){if(GridTerminalSystem.
CanAccess(ǯ)){if(ǯ.Status==DoorStatus.Closed||ǯ.Status==DoorStatus.Closing){ǯ.OpenDoor();}else{ǯ.ShowInTerminal=false;ǯ.ShowOnHUD
=false;}}}foreach(IMyGasTank Ȑ in ɲ){if(GridTerminalSystem.CanAccess(Ȑ)){Ȑ.Enabled=true;Ȑ.Stockpile=true;Ȑ.ShowInTerminal
=false;Ȑ.ShowOnHUD=false;}}foreach(IMyBatteryBlock ȑ in ɱ){if(GridTerminalSystem.CanAccess(ȑ)){ȑ.ChargeMode=ChargeMode.
Recharge;ȑ.ShowInTerminal=false;ȑ.ShowOnHUD=false;}}if(ɱ.Count>0){if(GridTerminalSystem.CanAccess(ɱ[0])){ɱ[0].ChargeMode=
ChargeMode.Auto;}}if(ɱ.Count==0){for(int Į=0;Į<ɸ.Count;Į++){if(GridTerminalSystem.CanAccess(ɱ[Į])){if(Į==0){ɸ[Į].Enabled=true;ɸ[Į]
.UseConveyorSystem=true;ɸ[Į].ShowInTerminal=false;ɸ[Į].ShowOnHUD=false;}else{ɸ[Į].Enabled=false;ɸ[Į].UseConveyorSystem=
true;ɸ[Į].ShowInTerminal=false;ɸ[Į].ShowOnHUD=false;}}}}else{foreach(IMyReactor Ȓ in ɸ){if(GridTerminalSystem.CanAccess(Ȓ)){
Ȓ.Enabled=false;Ȓ.UseConveyorSystem=true;Ȓ.ShowInTerminal=false;Ȓ.ShowOnHUD=false;}}}}void ȓ(){Ɂ=Ʉ;Ʉ=Ȱ.ȴ;foreach(
IMyAirtightHangarDoor Ȕ in Ɏ){if(GridTerminalSystem.CanAccess(Ȕ)){Ȕ.ShowInTerminal=false;Ȕ.ShowOnHUD=false;if(Ȕ.Status==DoorStatus.Closed||Ȕ.
Status==DoorStatus.Closing){Ȕ.OpenDoor();}else{}}}foreach(IMyEventControllerBlock m in ɝ){}foreach(IMyFlightMovementBlock m in
ɠ){if(GridTerminalSystem.CanAccess(m)){m.AlignToPGravity=false;m.CollisionAvoidance=false;m.FlightMode=FlightMode.OneWay;
m.SpeedLimit=ServerSpeedLimit;m.MinimalAltitude=100000f;m.PrecisionMode=false;}}foreach(IMyOffensiveCombatBlock m in ɞ){
if(GridTerminalSystem.CanAccess(m)){m.Enabled=false;m.UpdateTargetInterval=2;m.SetValue("CanTargetCharacters",true);m.
ApplyAction("SetAttackMode_EnemiesOnly");m.ApplyAction("SetTargetPriority_Largest");m.ApplyAction("SetTargetingGroup_Weapons");m.
ApplyAction("OffensiveCombatStayAtRange_FacingFront");m.SetValue("OffensiveCombatStayAtRange_MaximalDistance",25f);m.SetValue(
"OffensiveCombatStayAtRange_MinimalDistance",24f);}}foreach(IMyDefensiveCombatBlock m in Ɇ){if(GridTerminalSystem.CanAccess(m)){m.Enabled=false;m.FleeTrigger=
FleeTrigger.Always;m.CustomFleeCoordinates=new Vector3D(0,0,0);m.ApplyAction("SetAttackMode_EnemiesOnly");m.ApplyAction(
"SetTargetingGroup_Weapons");}}foreach(IMyPathRecorderBlock m in Ȧ){}foreach(IMyRemoteControl Ȍ in ɜ){}foreach(IMyThrust ȍ in ɚ){if(
GridTerminalSystem.CanAccess(ȍ)){ȍ.Enabled=true;}}foreach(IMyThrust Ȏ in ɛ){if(GridTerminalSystem.CanAccess(Ȏ)){Ȏ.Enabled=true;}}foreach(
IMyLightingBlock ȏ in ɳ){if(GridTerminalSystem.CanAccess(ȏ)){if(ȏ.BlockDefinition.SubtypeName.Equals("RotatingLightLarge")){ȏ.Enabled=
true;ȏ.Radius=30f;ȏ.BlinkIntervalSeconds=0;ȏ.BlinkLength=0f;ȏ.BlinkOffset=0f;ȏ.Color=RedAlertLightColor;}else if(ȏ.
BlockDefinition.SubtypeName.Equals("LargeBlockFrontLight")){ȏ.Enabled=true;ȏ.Color=SpotLightColor;ȏ.BlinkIntervalSeconds=0f;ȏ.
BlinkLength=0f;ȏ.BlinkOffset=0f;ȏ.Falloff=100f;ȏ.Intensity=5.0f;ȏ.Radius=160f;}else if(ȏ is IMyInteriorLight&&
InteriorLightsIndicateAlertLevel){ȏ.Color=RedAlertLightColor;ȏ.BlinkIntervalSeconds=0f;ȏ.BlinkLength=0f;ȏ.BlinkOffset=0f;ȏ.Falloff=5f;ȏ.Intensity=5.0f;ȏ
.Radius=160f;}else{ȏ.Color=DefaultInteriorLightsColor;ȏ.BlinkIntervalSeconds=0f;ȏ.BlinkLength=0f;ȏ.BlinkOffset=0f;ȏ.
Falloff=5f;ȏ.Intensity=5.0f;ȏ.Radius=160f;}}}foreach(IMyBatteryBlock ȑ in ɱ){if(GridTerminalSystem.CanAccess(ȑ)){ȑ.
ShowInTerminal=false;ȑ.ShowOnHUD=false;if(ȑ.CurrentStoredPower>=5){ȑ.ChargeMode=ChargeMode.Discharge;}else{ȑ.ChargeMode=ChargeMode.
Auto;}}}foreach(IMyReactor Ȓ in ɸ){if(GridTerminalSystem.CanAccess(Ȓ)){Ȓ.UseConveyorSystem=true;Ȓ.Enabled=true;Ȓ.
ShowInTerminal=false;Ȓ.ShowOnHUD=false;}}foreach(IMyGasTank Ȑ in ɲ){if(GridTerminalSystem.CanAccess(Ȑ)){Ȑ.Enabled=true;Ȑ.Stockpile=
false;Ȑ.ShowInTerminal=false;Ȑ.ShowOnHUD=false;}}foreach(IMyDoor ǯ in ɴ){if(GridTerminalSystem.CanAccess(ǯ)){ǯ.ShowInTerminal
=false;ǯ.ShowOnHUD=false;if(ǯ.Status==DoorStatus.Closed||ǯ.Status==DoorStatus.Closing){}else{ǯ.CloseDoor();}}}foreach(
IMyRadioAntenna ȕ in ɼ){if(GridTerminalSystem.CanAccess(ȕ)){ȕ.Enabled=AntennaeActiveYellowAlert;ȕ.ShowInTerminal=false;ȕ.ShowOnHUD=
false;}}foreach(IMyBeacon ɽ in ɮ){if(ș==null||!GridTerminalSystem.CanAccess(ș)){if(GridTerminalSystem.CanAccess(ɽ)){ɽ.Enabled
=BeaconActiveYellowAlertAndAbove;ɽ.Radius=5000;ș=ɽ;}}else{if(GridTerminalSystem.CanAccess(ɽ)&&(ɽ!=ș)){ɽ.Enabled=false;ɽ.
Radius=5000;}if(GridTerminalSystem.CanAccess(ɽ)&&(ɽ==ș)){ș.Enabled=BeaconActiveYellowAlertAndAbove;ɽ.Radius=5000;}}}foreach(
IMyShipWelder ʣ in ɏ){if(GridTerminalSystem.CanAccess(ʣ)){if(!ʣ.Enabled){ʣ.Enabled=true;ʣ.UseConveyorSystem=true;ʣ.ShowInTerminal=
false;ʣ.ShowOnHUD=false;}else{continue;}}}foreach(IMySoundBlock m in ɐ){if(GridTerminalSystem.CanAccess(m)){m.Enabled=false;}
}}void ʬ(){Ɂ=Ʉ;Ʉ=Ȱ.ȳ;foreach(IMyAirtightHangarDoor Ȕ in Ɏ){if(GridTerminalSystem.CanAccess(Ȕ)){Ȕ.ShowInTerminal=false;Ȕ.
ShowOnHUD=false;if(Ȕ.Status==DoorStatus.Closed||Ȕ.Status==DoorStatus.Closing){}else{Ȕ.CloseDoor();}}}foreach(
IMyEventControllerBlock m in ɝ){}foreach(IMyPathRecorderBlock m in Ȧ){if(GridTerminalSystem.CanAccess(m)){m.Enabled=false;}}foreach(
IMyOffensiveCombatBlock m in ɞ){if(GridTerminalSystem.CanAccess(m)){m.Enabled=false;m.UpdateTargetInterval=5;m.SelectedAttackPattern=3;m.
ApplyAction("SetAttackMode_EnemiesOnly");m.ApplyAction("SetTargetPriority_Largest");m.ApplyAction("SetTargetingGroup_PowerSystems")
;m.ApplyAction("OffensiveCombatIntercept_OverrideCollisionAvoidance");m.SetValue<long>(
"OffensiveCombatIntercept_GuidanceType",0);}}foreach(IMyFlightMovementBlock m in ɠ){if(GridTerminalSystem.CanAccess(m)){m.AlignToPGravity=false;m.
CollisionAvoidance=false;m.FlightMode=FlightMode.OneWay;m.MinimalAltitude=100000f;m.PrecisionMode=false;m.SpeedLimit=ServerSpeedLimit;}}
foreach(IMyDefensiveCombatBlock m in Ɇ){if(GridTerminalSystem.CanAccess(m)){m.Enabled=false;m.FleeTrigger=FleeTrigger.Always;m.
CustomFleeCoordinates=new Vector3D(0,0,0);m.ApplyAction("SetAttackMode_EnemiesOnly");m.ApplyAction("SetTargetingGroup_Weapons");}}foreach(
IMyRemoteControl Ȍ in ɜ){}foreach(IMyThrust ȍ in ɚ){if(GridTerminalSystem.CanAccess(ȍ)){ȍ.Enabled=false;}}foreach(IMyThrust Ȏ in ɛ){if(
GridTerminalSystem.CanAccess(Ȏ)){Ȏ.Enabled=true;}}foreach(IMyLightingBlock ȏ in ɳ){if(GridTerminalSystem.CanAccess(ȏ)){if(ȏ.
BlockDefinition.SubtypeName.Equals("RotatingLightLarge")&&ȏ is IMyReflectorLight){ȏ.Enabled=true;ȏ.Color=YellowAlertLightColor;ȏ.
BlinkIntervalSeconds=0;ȏ.BlinkLength=0f;ȏ.BlinkOffset=0f;ȏ.Falloff=5f;ȏ.Intensity=5.0f;ȏ.Radius=160f;}else if(ȏ.BlockDefinition.SubtypeName.
Equals("LargeBlockFrontLight")&&ȏ is IMyReflectorLight){ȏ.Enabled=true;ȏ.Color=SpotLightColor;ȏ.BlinkIntervalSeconds=0;ȏ.
BlinkLength=0f;ȏ.BlinkOffset=0f;ȏ.Falloff=100f;ȏ.Intensity=5.0f;ȏ.Radius=160f;}else if(ȏ is IMyInteriorLight&&
InteriorLightsIndicateAlertLevel){ȏ.Color=YellowAlertLightColor;ȏ.BlinkIntervalSeconds=0f;ȏ.BlinkLength=0f;ȏ.BlinkOffset=0f;ȏ.Falloff=5f;ȏ.Intensity=
5.0f;ȏ.Radius=160f;}else{if(ȏ is IMyReflectorLight&&!InteriorLightsIndicateAlertLevel){ȏ.Color=DefaultInteriorLightsColor;ȏ.
BlinkIntervalSeconds=0f;ȏ.BlinkLength=0f;ȏ.BlinkOffset=0f;ȏ.Falloff=5f;ȏ.Intensity=5.0f;ȏ.Radius=160f;}}}}foreach(IMyBatteryBlock ȑ in ɱ){if
(GridTerminalSystem.CanAccess(ȑ)){ȑ.Enabled=true;ȑ.ChargeMode=ChargeMode.Auto;}}foreach(IMyReactor Ȓ in ɸ){if(
GridTerminalSystem.CanAccess(Ȓ)){Ȓ.UseConveyorSystem=true;Ȓ.Enabled=true;}}foreach(IMyGasTank Ȑ in ɲ){if(GridTerminalSystem.CanAccess(Ȑ)){
Ȑ.Enabled=true;Ȑ.Stockpile=false;}}foreach(IMyDoor ǯ in ɴ){if(GridTerminalSystem.CanAccess(ǯ)){if(ǯ.Status==DoorStatus.
Closed||ǯ.Status==DoorStatus.Closing){ǯ.ShowInTerminal=false;ǯ.ShowOnHUD=false;}else{ǯ.CloseDoor();}}}foreach(IMyRadioAntenna
ȕ in ɼ){if(GridTerminalSystem.CanAccess(ȕ)){ȕ.Enabled=AntennaeActiveYellowAlert;ȕ.Radius=5000;}}foreach(IMyBeacon ɽ in ɮ)
{if(ș==null||!GridTerminalSystem.CanAccess(ș)){if(GridTerminalSystem.CanAccess(ɽ)){ɽ.Enabled=
BeaconActiveYellowAlertAndAbove;ɽ.Radius=5000;ș=ɽ;}}else{if(GridTerminalSystem.CanAccess(ɽ)&&(ɽ!=ș)){ɽ.Enabled=false;ɽ.Radius=5000;}if(
GridTerminalSystem.CanAccess(ɽ)&&(ɽ==ș)){ș.Enabled=BeaconActiveYellowAlertAndAbove;ɽ.Radius=5000;}}}foreach(IMyShipWelder ʣ in ɏ){if(
GridTerminalSystem.CanAccess(ʣ)){if(!ʣ.Enabled){ʣ.Enabled=true;ʣ.UseConveyorSystem=true;ʣ.ShowInTerminal=false;ʣ.ShowOnHUD=false;}else{
continue;}}}foreach(IMySoundBlock m in ɐ){if(GridTerminalSystem.CanAccess(m)){m.Enabled=false;}}}void ʭ(){Ɂ=Ʉ;Ʉ=Ȱ.ȷ;foreach(
IMyLightingBlock ȏ in ɳ){if(GridTerminalSystem.CanAccess(ȏ)){if(ȏ.BlockDefinition.SubtypeName.Equals("RotatingLightLarge")){ȏ.Color=
RedAlertLightColor;ȏ.BlinkIntervalSeconds=0;ȏ.BlinkLength=0f;ȏ.BlinkOffset=0f;ȏ.Falloff=5f;ȏ.Intensity=5.0f;ȏ.Radius=160f;}if(ȏ.
BlockDefinition.SubtypeName.Equals("LargeBlockFrontLight")){ȏ.Color=SpotLightColor;ȏ.BlinkIntervalSeconds=0;ȏ.BlinkLength=0f;ȏ.
BlinkOffset=0f;ȏ.Falloff=5f;ȏ.Intensity=5.0f;ȏ.Radius=160f;}if(ȏ is IMyInteriorLight){ȏ.Color=RedAlertLightColor;}}}}void ʮ(){Ɂ=Ʉ;Ʉ
=Ȱ.Ȳ;foreach(IMyAirtightHangarDoor Ȕ in Ɏ){if(GridTerminalSystem.CanAccess(Ȕ)){Ȕ.ShowInTerminal=false;Ȕ.ShowOnHUD=false;
if(Ȕ.Status==DoorStatus.Open){}else{if(Ȕ.Status==DoorStatus.Closed||Ȕ.Status==DoorStatus.Closing){Ȕ.OpenDoor();}}}}foreach
(IMyEventControllerBlock m in ɝ){if(GridTerminalSystem.CanAccess(m)){m.Enabled=false;m.IsAndModeEnabled=false;m.
IsLowerOrEqualCondition=true;m.Threshold=RedAlertDamageThreshold;}}foreach(IMyPathRecorderBlock m in Ȧ){m.Enabled=false;}foreach(
IMyFlightMovementBlock m in ɠ){if(GridTerminalSystem.CanAccess(m)){m.Enabled=false;m.AlignToPGravity=false;m.CollisionAvoidance=false;m.
FlightMode=FlightMode.OneWay;m.MinimalAltitude=100000f;m.PrecisionMode=false;m.SpeedLimit=350f;}}foreach(IMyDefensiveCombatBlock m
in Ɇ){if(GridTerminalSystem.CanAccess(m)){m.Enabled=false;m.FleeTrigger=FleeTrigger.Always;m.CustomFleeCoordinates=new
Vector3D(0,0,0);m.ApplyAction("SetAttackMode_EnemiesOnly");m.ApplyAction("SetTargetingGroup_Weapons");}}foreach(
IMyOffensiveCombatBlock m in ɞ){if(GridTerminalSystem.CanAccess(m)){m.Enabled=false;m.UpdateTargetInterval=2;m.SetValue("CanTargetCharacters",
true);m.ApplyAction("SetAttackMode_EnemiesOnly");m.ApplyAction("SetTargetPriority_Closest");m.ApplyAction(
"SetTargetingGroup_Weapons");m.ApplyAction("OffensiveCombatStayAtRange_FacingFront");m.SetValue("OffensiveCombatStayAtRange_MaximalDistance",25f);m
.SetValue("OffensiveCombatStayAtRange_MinimalDistance",24f);}}foreach(IMyThrust ȍ in ɚ){if(GridTerminalSystem.CanAccess(ȍ
)){ȍ.Enabled=HydrogenThrustONstandardCruising;}}foreach(IMyThrust Ȏ in ɛ){if(GridTerminalSystem.CanAccess(Ȏ)){Ȏ.Enabled=
true;}}foreach(IMyLightingBlock ȏ in ɳ){if(GridTerminalSystem.CanAccess(ȏ)){if(ȏ.BlockDefinition.SubtypeName.Equals(
"RotatingLightLarge")&&ȏ is IMyReflectorLight){ȏ.Enabled=false;ȏ.Color=StandardAlertLightColor;ȏ.BlinkIntervalSeconds=0f;ȏ.BlinkLength=0f;ȏ.
BlinkOffset=0f;ȏ.Falloff=5f;ȏ.Intensity=5.0f;ȏ.Radius=30f;}else if(ȏ.BlockDefinition.SubtypeName.Equals("LargeBlockFrontLight")&&ȏ
is IMyReflectorLight){ȏ.Enabled=true;ȏ.Color=SpotLightColor;ȏ.BlinkIntervalSeconds=0f;ȏ.BlinkLength=0f;ȏ.BlinkOffset=0f;ȏ.
Falloff=5f;ȏ.Intensity=5.0f;ȏ.Radius=160f;}else if(ȏ is IMyInteriorLight&&InteriorLightsIndicateAlertLevel){ȏ.Color=
StandardAlertLightColor;ȏ.BlinkIntervalSeconds=0f;ȏ.BlinkLength=0f;ȏ.BlinkOffset=0f;ȏ.Falloff=5f;ȏ.Intensity=5.0f;ȏ.Radius=160f;}else{ȏ.Color=
DefaultInteriorLightsColor;ȏ.BlinkIntervalSeconds=0f;ȏ.BlinkLength=0f;ȏ.BlinkOffset=0f;ȏ.Falloff=5f;ȏ.Intensity=5.0f;ȏ.Radius=160f;}}}foreach(
IMyBatteryBlock ȑ in ɱ){if(GridTerminalSystem.CanAccess(ȑ)){if(ɸ.Count>0){ȑ.Enabled=true;ȑ.ChargeMode=ChargeMode.Recharge;}else{ȑ.
Enabled=true;ȑ.ChargeMode=ChargeMode.Auto;}}}foreach(IMyReactor Ȓ in ɸ){if(GridTerminalSystem.CanAccess(Ȓ)){Ȓ.UseConveyorSystem
=true;Ȓ.Enabled=true;}}foreach(IMyGasTank Ȑ in ɲ){if(GridTerminalSystem.CanAccess(Ȑ)){Ȑ.Enabled=true;Ȑ.Stockpile=false;}}
foreach(IMyDoor ǯ in ɴ){if(GridTerminalSystem.CanAccess(ǯ)){ǯ.ShowInTerminal=false;ǯ.ShowOnHUD=false;}}foreach(IMyRadioAntenna
ȕ in ɼ){if(GridTerminalSystem.CanAccess(ȕ)){ȕ.Enabled=false;ȕ.EnableBroadcasting=true;ȕ.HudText=Me.CubeGrid.CustomName;ȕ.
Radius=5000;}}foreach(IMyBeacon ɽ in ɮ){if(GridTerminalSystem.CanAccess(ɽ)){ɽ.Enabled=false;ɽ.Radius=5000;ɽ.HudText=Me.
CubeGrid.CustomName;}}foreach(IMyShipWelder ʣ in ɏ){if(GridTerminalSystem.CanAccess(ʣ)){if(!ʣ.Enabled){ʣ.Enabled=false;ʣ.
UseConveyorSystem=true;ʣ.ShowInTerminal=false;ʣ.ShowOnHUD=false;}else{continue;}}}foreach(IMyButtonPanel ʤ in ɉ){if(GridTerminalSystem.
CanAccess(ʤ)){ʤ.AnyoneCanUse=false;}}foreach(IMySoundBlock m in ɐ){if(GridTerminalSystem.CanAccess(m)){m.Enabled=false;}}}void ʥ(
IMyTextSurface ʓ){ʓ.ContentType=ContentType.SCRIPT;ʓ.Script="None";Ǵ=ʓ.DrawFrame();ǵ=ǳ.Center;Ǽ=new MySprite(){Type=SpriteType.TEXTURE
,Data="Grid",Position=ǵ,Size=ʓ.TextureSize*2,Color=Color.OrangeRed.Alpha(0.90f),Alignment=TextAlignment.CENTER,
RotationOrScale=3/4f};Ǵ.Add(Ǽ);Ǽ=new MySprite(){Type=SpriteType.TEXTURE,Data="LCD_Emote_Suspicious_Right",Position=ǵ,Size=ǳ.Size/3,
Alignment=TextAlignment.CENTER};Ǵ.Add(Ǽ);ǵ+=Ƿ;Ǽ=new MySprite{Type=SpriteType.TEXT,Data="<-- LAST JUMP DRIVE! -->",Position=ǵ,
RotationOrScale=0.9f,Color=Color.GreenYellow,Alignment=TextAlignment.CENTER,FontId="Red"};Ǵ.Add(Ǽ);ǵ+=Ƿ;Ǽ=new MySprite{Type=SpriteType.
TEXT,Data="<-- JUMP AWAY NOW! -->",Position=ǵ,RotationOrScale=0.9f,Color=Color.GreenYellow,Alignment=TextAlignment.CENTER,
FontId="Red"};Ǵ.Add(Ǽ);Ǵ.Dispose();}void ʦ(IMyTextSurface ʓ){ʓ.ScriptBackgroundColor=Color.Black;ʓ.ContentType=ContentType.
SCRIPT;ʓ.Script="None";Ǵ=ʓ.DrawFrame();ǵ=ǳ.Center;Ǽ=new MySprite(){Type=SpriteType.TEXTURE,Data="Grid",Position=ǵ,Size=ʓ.
TextureSize*2,Color=Color.OrangeRed.Alpha(0.90f),Alignment=TextAlignment.CENTER,RotationOrScale=3/4f};Ǵ.Add(Ǽ);Ǽ=new MySprite(){
Type=SpriteType.TEXTURE,Data="LCD_Emote_Dead",Position=ǵ,Size=ǳ.Size/3,Alignment=TextAlignment.CENTER};Ǵ.Add(Ǽ);ǵ+=Ƿ;Ǽ=new
MySprite{Type=SpriteType.TEXT,Data="<-- NO REACTORS! -->",Position=ǵ,RotationOrScale=0.9f,Color=Color.IndianRed,Alignment=
TextAlignment.CENTER,FontId="Red"};Ǵ.Add(Ǽ);Ǵ.Dispose();}void ʧ(IMyTextSurface ʓ){ʓ.ScriptBackgroundColor=Color.Black;ʓ.ContentType=
ContentType.SCRIPT;ʓ.Script="None";Ǵ=ʓ.DrawFrame();ǵ=ʓ.TextureSize/2f;ǝ.Position=ǵ;Ǵ.Add(ǝ);Ǽ=ǣ;Ǵ.Add(Ǽ);ǵ+=Ƿ;Ǽ=Ȗ;Ǵ.Add(Ǽ);Ǵ.
Dispose();}void ʨ(IMyTextSurface ʓ,MyDetectedEntityInfo ʩ){ʓ.ScriptBackgroundColor=Color.Black;ʓ.ContentType=ContentType.SCRIPT
;ʓ.Script="None";ǵ=ʓ.TextureSize/2f;Ǵ=ʓ.DrawFrame();ǵ+=Ǻ;ǵ=ʓ.TextureSize/2f;ǝ.Position=ǵ;Ǵ.Add(ǝ);ǵ+=Ǹ;ǥ.Size=ʓ.
TextureSize/4f;ǥ.Position=ǵ;Ǵ.Add(ǥ);ǵ+=Ǻ;ǭ.Position=ǵ;Ǵ.Add(ǭ);ǵ+=Ǻ;Ȋ.Position=ǵ;Ȋ.Size=ǹ;Ȋ.Color=Color.Yellow;ǘ=(ʩ.Name!=null?ʩ.
Name:"Unknown name! (null)");Ǚ=(ʩ.Type.ToString());ǚ=(ʩ.Relationship.ToString());Ȋ.Data=
$"Contact: {ǘ}\nGridType: {Ǚ}\nGridRelationship: {ǚ}";Ǵ.Add(Ȋ);Ǵ.Dispose();}void ʪ(IMyTextSurface ʓ){ʓ.ScriptBackgroundColor=Color.Black;ʓ.ContentType=ContentType.SCRIPT;ʓ.
Script="None";Ǵ=ʓ.DrawFrame();ǵ=ʓ.TextureSize/2f;ǝ.Position=ǵ;Ǵ.Add(ǝ);ǵ+=Ǹ;ǥ.Size=ʓ.TextureSize/4f;ǥ.Position=ǵ;Ǵ.Add(ǥ);ǵ+=Ǻ
;ǭ.Position=ǵ;Ǵ.Add(ǭ);ǵ+=Ǻ;Ȋ.Position=ǵ;Ȋ.Size=ǹ;Ȋ.Color=Color.Yellow;Ȋ.Data="No targets!";Ǵ.Add(Ȋ);Ǵ.Dispose();}void ʢ(
IMyTextSurface ʓ){ʓ.ScriptBackgroundColor=Color.Black;ʓ.ContentType=ContentType.SCRIPT;ʓ.Script="None";Ǵ=ʓ.DrawFrame();ǵ=ʓ.TextureSize
/2f;ǝ.Position=ǵ;Ǵ.Add(ǝ);ǧ.Position=ǵ;ǧ.Size=ʓ.TextureSize/3f;Ǵ.Add(ǧ);ǻ.Position=ǵ+Ƿ;Ǵ.Add(ǻ);Ǵ.Dispose();Ǽ=ǝ;Ǵ.Add(Ǽ);
Ǽ=ǥ;Ǵ.Add(Ǽ);ǵ+=Ƿ;Ǽ=ǻ;Ǵ.Add(Ǽ);Ǵ.Dispose();}void ʯ(IMyTextSurface ʓ,bool Ȟ){ʓ.ScriptBackgroundColor=Color.Black;ʓ.
ContentType=ContentType.SCRIPT;ʓ.Script="None";Ǵ=ʓ.DrawFrame();ǵ=ʓ.TextureSize/2f;ǝ.Position=ǵ;Ǵ.Add(ǝ);ǩ.Position=ǵ;ǩ.Size=ʓ.
TextureSize/3f;Ǵ.Add(ǩ);Ǫ.Position=ǵ+Ǻ;Ǫ.Data=ȧ.ToString();Ǵ.Add(Ǫ);Ǵ.Dispose();}void ʵ(IMyTextSurface ʓ){ʓ.ScriptBackgroundColor=
Color.Black;ʓ.ContentType=ContentType.SCRIPT;ʓ.Script="None";Ǵ=ʓ.DrawFrame();ǵ=ʓ.TextureSize/2f;ǝ.Position=ǵ;Ǵ.Add(ǝ);Ǟ.
Position=ǵ;Ǟ.Size=ʓ.TextureSize/3f;Ǵ.Add(Ǟ);ǟ.Position=ǵ+Ƿ;Ǵ.Add(ǟ);Ǵ.Dispose();}void ʶ(IMyTextSurface ʓ){ʓ.
ScriptBackgroundColor=Color.Black;ʓ.ContentType=ContentType.SCRIPT;ʓ.Script="None";Ǵ=ʓ.DrawFrame();ǵ=ʓ.TextureSize/2f;ǝ.Position=ǵ;Ǵ.Add(ǝ);Ǡ
.Position=ǵ;Ǡ.Size=ʓ.TextureSize/3f;Ǵ.Add(Ǡ);ǡ.Position=ǵ+Ƿ;Ǵ.Add(ǡ);Ǵ.Dispose();}void ʷ(IMyTextSurface ʓ){ʓ.ContentType=
ContentType.SCRIPT;ʓ.Script="None";Ǵ=ʓ.DrawFrame();ǵ=ʓ.TextureSize/2f;ǝ.Position=ǵ;Ǵ.Add(ǝ);Ǣ.Position=ǵ;Ǣ.Size=ʓ.TextureSize/3f;Ǵ.
Add(Ǣ);Ǥ.Position=ǵ+Ƿ;Ǵ.Add(Ǥ);Ǵ.Dispose();}void ʸ(IMyTextSurface ʓ){ʓ.ContentType=ContentType.SCRIPT;ʓ.Script="None";Ǵ=ʓ.
DrawFrame();ǵ=ʓ.TextureSize/2f;ǝ.Position=ǵ;Ǵ.Add(ǝ);ǥ.Position=ǵ;ǥ.Size=ʓ.TextureSize/3f;Ǵ.Add(ǥ);Ǧ.Position=ǵ+Ƿ;Ǵ.Add(Ǧ);Ǵ.
Dispose();}void ʹ(IMyTextSurface ʓ){ʓ.ContentType=ContentType.SCRIPT;ʓ.Script="None";Ǵ=ʓ.DrawFrame();ǵ=ʓ.TextureSize/2f;ǝ.
Position=ǵ;Ǵ.Add(ǝ);Ǩ.Position=ǵ;Ǩ.Size=ʓ.TextureSize/3f;Ǵ.Add(Ǩ);ǧ.Position=ǵ+Ƿ;Ǵ.Add(ǧ);Ǵ.Dispose();}void ʻ(IMyTextSurface ʓ){
ʓ.ScriptBackgroundColor=Color.Black;ʓ.ContentType=ContentType.SCRIPT;ʓ.Script="None";Ǵ=ʓ.DrawFrame();ǵ=ʓ.TextureSize/2f;ǝ
.Position=ǵ;Ǵ.Add(ǝ);ǩ.Position=ǵ;ǩ.Size=ʓ.TextureSize/3f;Ǵ.Add(ǩ);Ǫ.Position=ǵ+Ƿ+Ƿ;Ǫ.Data=ȧ.ToString();Ǵ.Add(Ǫ);ǫ.
Position=ǵ+Ƿ+Ƿ+Ƿ;Ǵ.Add(ǫ);Ǵ.Dispose();}void ʼ(List<IMyCockpit>Ư){for(int Į=0;Į<Ư.Count;Į++){if(GridTerminalSystem.CanAccess(Ư[Į]
)){Ȉ=Ư[Į].SurfaceCount;for(int ʄ=0;ʄ<Ư[Į].SurfaceCount;ʄ++){if(GridTerminalSystem.CanAccess(Ư[Į])){ǽ=null;Ǿ=null;ǿ=null;Ȁ
=null;ȁ=null;Ȃ=null;if(Ȉ>0){ǽ=Ư[Į].GetSurface(0);ǽ.ContentType=ContentType.SCRIPT;ǽ.Script="TSS_ArtificialHorizon";ǽ.
ScriptBackgroundColor=Color.Black;ǽ.ScriptForegroundColor=Color.DarkOrange;}if(Ȉ>1){Ǿ=Ư[Į].GetSurface(1);Ǿ.ContentType=ContentType.SCRIPT;Ǿ.
Script="TSS_Velocity";Ǿ.ScriptBackgroundColor=Color.Black;Ǿ.ScriptForegroundColor=Color.DarkOrange;}if(Ȉ>2){ǿ=Ư[Į].GetSurface(
2);ǿ.ContentType=ContentType.SCRIPT;ǿ.Script="TSS_EnergyHydrogen";ǿ.ScriptBackgroundColor=Color.Black;ǿ.
ScriptForegroundColor=Color.DarkOrange;}if(Ȉ>3){Ȁ=Ư[Į].GetSurface(3);Ȁ.ContentType=ContentType.SCRIPT;Ȁ.Script="None";Ȁ.ScriptBackgroundColor
=Color.Black;Ȁ.ScriptForegroundColor=Color.DarkOrange;ʯ(Ȁ,debug);}if(Ȉ>4){ȁ=Ư[Į].GetSurface(4);if(ȁ.ContentType!=
ContentType.TEXT_AND_IMAGE){ȁ.ContentType=ContentType.TEXT_AND_IMAGE;}ȁ.TextPadding=0f;ȁ.BackgroundColor=Color.Black;ȁ.FontColor=
Color.Aquamarine;ȁ.FontSize=1.0f;ȁ.Alignment=TextAlignment.LEFT;ȁ.WriteText("Ship Alert Status: "+Ț(Ʉ)+"\n"+
$"AlertTriggeredByEvent: "+ȿ+"\n"+$"WCThreatCount:"+ȩ+"\n"+$"WCThreatsListValue"+ǲ.Count+"\n");ȁ.WriteText($"Shield Online? {Ȼ.Ƅ()}\n"+
$"GridHasShield? {Ȼ.ƈ(Me.CubeGrid)}\n"+$"GridShieldOnline? {Ȼ.Ɗ(Me.CubeGrid)}\n"+$"Shield Percentage: {ȧ}\n",true);ȁ.WriteText("TIME: "+System.DateTime.Now.
ToString()+":\n",true);if(ȩ<=0){ȁ.WriteText("NO TARGETS at time "+System.DateTime.Now.ToString(),true);}else{if(ǲ.Count>0){
foreach(MyDetectedEntityInfo ʽ in ǲ.Keys){if(ʽ.Name==null){Ȫ="TARGET NAMELESS!";}else{Ȫ=ʽ.Name;}if(ʽ.Relationship==null){ȫ=
"TARGET HAS NO RELATIONSHIP!";}else{ȫ=ʽ.Relationship.ToString();}if(ʽ.Velocity==null){ȭ=(-1).ToString();}else{ȭ=ʽ.Velocity.ToString();}if(ʽ.Position
==null){Ȭ=(-1).ToString();}else{Ȭ=ʽ.Position.ToString();}if(debug){Me.CustomData+=("Target Name: "+Ȫ+" \nTarget Velocity"+
ȭ+" \nTarget Position"+Ȭ);ȁ.WriteText("Target Name: "+Ȫ+" \nTarget Velocity"+ȭ+" \nTarget Position"+Ȭ,true);}else{Me.
CustomData+=("Target Name: "+Ȫ+" \nTarget Velocity"+ȭ+" \nTarget Position"+Ȭ);ȁ.WriteText("Target Name: "+Ȫ+" \nTarget Velocity"+ȭ
+" \nTarget Position"+Ȭ,true);}}}}}if(Ȉ>5){Ȃ=Ư[Į].GetSurface(5);Ȃ.ContentType=ContentType.SCRIPT;Ȃ.Script="None";Ȃ.
ScriptBackgroundColor=Color.Black;Ȃ.ScriptForegroundColor=Color.DarkOrange;switch(Ʉ){case Ȱ.Ȳ:ʶ(Ȃ);break;case Ȱ.ȳ:ʷ(Ȃ);break;case Ȱ.ȴ:ʸ(Ȃ);
break;case Ȱ.ȱ:ʵ(Ȃ);break;default:ʢ(Ȃ);break;}}}}}}}void ʾ(List<IMyMotorAdvancedStator>ʿ){foreach(IMyMotorAdvancedStator ˀ in
ʿ){if(ˀ.IsFunctional&&!ˀ.IsAttached){{try{ˀ.ApplyAction("AddSmallRotorTopPart");}catch(Exception e){ȅ.Enqueue(
"DEBUG: MonitorAdvStators threw exception!");ȅ.Enqueue(e.ToString());}}}}}void ˁ(s ç){if((ȧ<=RedAlertDamageThreshold)){if(!ȿ&&ȩ>0){ȿ=true;Ɂ=Ʉ;ȅ.Enqueue(
$"RedAlert triggered hostile and by shield percentage of {ȧ}");if(debug){ȅ.Enqueue($"RedAlertTriggeredByHostile: {ȿ}");ȅ.Enqueue(
$"!RedAlertTriggeredByHostile && LastAlertLevel != AlertLevel.redalert && WCThreatCount > 0:"+(!ȿ&&Ɂ!=Ȱ.ȴ&&ȩ>0));}switch(Ʉ){case Ȱ.Ȳ:ȓ();break;case Ȱ.ȳ:ȓ();break;case Ȱ.ȴ:break;case Ȱ.ȷ:ȓ();break;case Ȱ.ȶ:ȓ();
break;case Ȱ.ȱ:ȓ();break;default:ȓ();break;}return;}if(!ȿ&&ȩ==0){Ʉ=Ɂ;Ɂ=Ʉ;if(debug){ȅ.Enqueue($"RedAlertTriggeredByEvent: {ȿ}"
);ȅ.Enqueue($"!RedAlertTriggeredByEvent && LastAlertLevel != AlertLevel.redalert && WCThreatCount > 0:"+(!ȿ&&Ɂ!=Ȱ.ȴ&&ȩ>0)
);}switch(Ʉ){case Ȱ.Ȳ:break;case Ȱ.ȳ:break;case Ȱ.ȴ:ʬ();break;case Ȱ.ȷ:break;case Ȱ.ȶ:break;case Ȱ.ȱ:break;default:break;
}return;}ʻ(Me.GetSurface(0));}else{if(debug){ȅ.Enqueue($"RedAlertTriggeredByEvent: {ȿ}");ȅ.Enqueue(
$"GridShieldOnline(Me.CubeGrid) && (ShipShieldPercent <= RedAlertDamageThreshold || WCThreatCount > 0:"+(ç.Ɗ(Me.CubeGrid)&&(ȧ<=RedAlertDamageThreshold||ȩ>0)));}if(!ȿ&&ȩ==0){if(debug){ȅ.Enqueue(
$"RedAlertTriggeredByEvent: {ȿ}");ȅ.Enqueue($"!RedAlertTriggeredByEvent && WCThreatCount == 0:"+(!ȿ&&ȩ==0));}switch(Ʉ){case Ȱ.ȴ:break;case Ȱ.ȳ:break;
case Ȱ.Ȳ:break;case Ȱ.ȶ:break;case Ȱ.ȱ:break;default:break;}return;}if(ȿ&&ȩ==0){if(debug){ȅ.Enqueue(
$"RedAlertTriggeredByEvent: {ȿ}");ȅ.Enqueue($"RedAlertTriggeredByEvent && WCThreatCount == 0:"+(ȿ&&ȩ==0));}ȿ=false;Ʉ=Ɂ;switch(Ʉ){case Ȱ.ȴ:ʬ();break;case
Ȱ.ȳ:break;case Ȱ.Ȳ:break;case Ȱ.ȶ:break;case Ȱ.ȱ:ȥ();break;}return;}if(ȿ&&ȩ>0){if(debug){ȅ.Enqueue(
$"RedAlertTriggeredByEvent: {ȿ}");ȅ.Enqueue($"RedAlertTriggeredByEvent && WCThreatCount > 0:"+(ȿ&&ȩ>0));}switch(Ʉ){case Ȱ.ȴ:break;case Ȱ.ȳ:break;case Ȱ.
Ȳ:ʬ();break;case Ȱ.ȶ:ȓ();break;case Ȱ.ȱ:ȓ();break;default:break;}return;}if(!ȿ&&ȩ>0){if(debug){ȅ.Enqueue(
$"RedAlertTriggeredByEvent: {ȿ}");ȅ.Enqueue($"!RedAlertTriggeredByEvent && WCThreatCount > 0:"+(!ȿ&&ȩ>0));}switch(Ʉ){case Ȱ.ȴ:break;case Ȱ.ȳ:ȓ();break;
case Ȱ.Ȳ:ȓ();break;case Ȱ.ȶ:break;case Ȱ.ȱ:ȓ();break;}return;}}}void ˆ(List<IMySensorBlock>ˇ){foreach(IMySensorBlock ʺ in ˇ)
{if(ʺ.IsWorking){if(ʺ.LastDetectedEntity.Relationship==MyRelationsBetweenPlayerAndBlock.Enemies){if(debug){ȅ.Enqueue(
"DEBUG: Red Alert triggered due to hostile detected via sensor!");}ȿ=true;ȓ();}}}}void ʴ(List<IMyJumpDrive>ɷ){if(ɷ.Count==0){foreach(IMyTextSurfaceProvider ʇ in ɵ){IMyTextSurface ʰ=ʇ.
GetSurface(0);if(ʰ!=null){ʧ(ʰ);}}}if(ɷ.Count==1){foreach(IMyTextSurfaceProvider ʇ in ɵ){IMyTextSurface ʰ=ʇ.GetSurface(0);if(ʰ!=
null){ʥ(ʰ);}}}Ⱦ=5000;for(int Į=0;Į<ɷ.Count;Į++){if(GridTerminalSystem.CanAccess(ɷ[Į])){ȗ=ɷ[Į];if(!ȗ.Enabled){if(ȗ==Ș){
continue;}if(!ȗ.CustomName.Contains("OL-Jump Drive")){ȗ.CustomName="";ȗ.CustomName=("OL-Jump Drive"+Ⱦ);ȗ.Enabled=true;ȗ.Recharge
=true;ȗ.JumpDistanceMeters=Ⱦ;Ⱦ+=5000;}}else{if(ȗ==Ș){continue;}ȗ.CustomName="";ȗ.CustomName=("OL-Jump Drive"+Ⱦ);ȗ.
Recharge=true;ȗ.JumpDistanceMeters=Ⱦ;Ⱦ+=5000;}}}Ⱦ=0;}void ʱ(List<IMyShipController>ʲ){Ȩ=false;foreach(IMyShipController ʳ in ʲ){
if(GridTerminalSystem.CanAccess(ʳ)&&ʳ.IsUnderControl){Ȩ=true;}}switch(Ȩ){case true:switch(Ʉ){case Ȱ.Ȳ:foreach(
IMyDefensiveCombatBlock ɾ in Ɇ){ɾ.Enabled=false;ɾ.ApplyAction("ActivateBehavior_Off");}foreach(IMyOffensiveCombatBlock ʀ in ɞ){ʀ.Enabled=false;
ʀ.ApplyAction("ActivateBehavior_Off");}foreach(IMyFlightMovementBlock ɿ in ɠ){ɿ.Enabled=false;ɿ.ApplyAction(
"ActivateBehavior_Off");}break;case Ȱ.ȳ:foreach(IMyDefensiveCombatBlock ɾ in Ɇ){ɾ.Enabled=false;ɾ.ApplyAction("ActivateBehavior_Off");}foreach
(IMyOffensiveCombatBlock ʀ in ɞ){ʀ.Enabled=false;ʀ.ApplyAction("ActivateBehavior_Off");}foreach(IMyFlightMovementBlock ɿ
in ɠ){ɿ.Enabled=false;ɿ.ApplyAction("ActivateBehavior_Off");}break;case Ȱ.ȴ:foreach(IMyDefensiveCombatBlock ɾ in Ɇ){ɾ.
Enabled=false;ɾ.ApplyAction("ActivateBehavior_Off");}foreach(IMyOffensiveCombatBlock ʀ in ɞ){ʀ.Enabled=false;ʀ.ApplyAction(
"ActivateBehavior_Off");}foreach(IMyFlightMovementBlock ɿ in ɠ){ɿ.Enabled=false;ɿ.ApplyAction("ActivateBehavior_Off");}break;case Ȱ.ȱ:foreach(
IMyDefensiveCombatBlock ɾ in Ɇ){ɾ.Enabled=false;ɾ.ApplyAction("ActivateBehavior_Off");}foreach(IMyOffensiveCombatBlock ʀ in ɞ){ʀ.Enabled=false;
ʀ.ApplyAction("ActivateBehavior_Off");}foreach(IMyFlightMovementBlock ɿ in ɠ){ɿ.Enabled=false;ɿ.ApplyAction(
"ActivateBehavior_Off");}break;}break;case false:switch(Ʉ){case Ȱ.Ȳ:foreach(IMyDefensiveCombatBlock ɾ in Ɇ){if(GridTerminalSystem.CanAccess(ɾ)
){ɾ.Enabled=false;ɾ.ApplyAction("ActivateBehavior_On");}}foreach(IMyOffensiveCombatBlock ʀ in ɞ){if(GridTerminalSystem.
CanAccess(ʀ)){ʀ.Enabled=true;ʀ.ApplyAction("ActivateBehavior_Off");}}foreach(IMyFlightMovementBlock ɿ in ɠ){if(GridTerminalSystem
.CanAccess(ɿ)){ɿ.Enabled=true;ɿ.ApplyAction("ActivateBehavior_Off");}}break;case Ȱ.ȳ:foreach(IMyDefensiveCombatBlock ɾ in
Ɇ){ɾ.Enabled=false;ɾ.ApplyAction("ActivateBehavior_On");}foreach(IMyOffensiveCombatBlock ʀ in ɞ){ʀ.Enabled=true;ʀ.
ApplyAction("ActivateBehavior_Off");}foreach(IMyFlightMovementBlock ɿ in ɠ){ɿ.Enabled=true;ɿ.ApplyAction("ActivateBehavior_On");}
break;case Ȱ.ȴ:foreach(IMyDefensiveCombatBlock ɾ in Ɇ){ɾ.Enabled=true;ɾ.ApplyAction("ActivateBehavior_On");}foreach(
IMyOffensiveCombatBlock ʀ in ɞ){ʀ.Enabled=false;ʀ.ApplyAction("ActivateBehavior_Off");}foreach(IMyFlightMovementBlock ɿ in ɠ){ɿ.Enabled=false;ɿ
.ApplyAction("ActivateBehavior_Off");}break;case Ȱ.ȱ:foreach(IMyDefensiveCombatBlock ɾ in Ɇ){ɾ.Enabled=false;ɾ.
ApplyAction("ActivateBehavior_Off");}foreach(IMyOffensiveCombatBlock ʀ in ɞ){ʀ.Enabled=true;ʀ.ApplyAction("ActivateBehavior_On");}
foreach(IMyFlightMovementBlock ɿ in ɠ){ɿ.Enabled=true;ɿ.ApplyAction("ActivateBehavior_On");}break;}break;}}void ʁ(
IMyTerminalBlock ȟ,List<IMyDoor>ʂ){if(debug){ȅ.Enqueue("DEBUG: MonitorGroupGunDoors is using the following railgun:");}if(ȟ!=null&&Ⱥ.W(ȟ
,0,true,true)){if(debug){Echo("DEBUG: MonitorGunDoors: railgun reports READY to fire! Opening door!");}foreach(IMyDoor ǯ
in ʂ){if(ǯ.Status==DoorStatus.Closed||ǯ.Status==DoorStatus.Closing){ǯ.OpenDoor();}}}else{if(debug){Echo(
"DEBUG: MonitorGunDoors: railgun reports NOT READY to fire! Closing door!");}foreach(IMyDoor ǯ in ɘ){if(ǯ.Status==DoorStatus.Open||ǯ.Status==DoorStatus.Opening){ǯ.CloseDoor();}}}}void ʃ(List<
IMyReactor>ɸ){if(ɸ.Count==1){for(int Į=0;Į<ɹ.Count;Į++){for(int ʄ=0;ʄ<ɹ[Į].SurfaceCount;ʄ++){ʥ(ɹ[Į].GetSurface(ʄ));}}}else if(ɸ.
Count==0){for(int Į=0;Į<ɹ.Count;Į++){for(int ʄ=0;ʄ<ɹ[Į].SurfaceCount;ʄ++){ʧ(ɹ[Į].GetSurface(ʄ));}}}}void ʅ(List<
IMyTextSurfaceProvider>ʆ){foreach(IMyTextSurfaceProvider ʇ in ʆ){for(int Į=0;Į<ʇ.SurfaceCount;Į++){IMyTextSurface ʈ=ʇ.GetSurface(Į);ʈ.
BackgroundColor=Color.Black;ʈ.FontColor=Color.PaleGreen;ʈ.FontSize=1.2f;ʈ.ContentType=ContentType.TEXT_AND_IMAGE;ʈ.WriteText(
$"ROLLING LOG >> \n"+Ƚ+"\n>>\nServer Time:"+DateTime.Now.ToShortTimeString()+"\nMinus-1 Time:"+DateTime.Now.Subtract(TimeSpan.FromHours(1))+
"\n",false);ʈ.WriteText($"Current Grid Name: {Me.CubeGrid.DisplayName}\nAlert Level: {Ț(Ʉ)}\nLast Alert Level: {Ț(Ɂ)}\nShield Percent: {ȧ}\nWeaponcore Last Detected Target: {Ȫ}\nTarget Relationship: {ȫ.ToString()}"
,true);}}}void ʉ(List<IMyDecoy>ʊ){foreach(IMyDecoy ʋ in ʊ){}}Program(){ȯ=Me.GetSurface(0);ǳ=new RectangleF(ȯ.TextureSize-
ȯ.SurfaceSize/2f,ȯ.SurfaceSize);ȃ=new List<Vector3>{Vector3.Zero,Vector3.Zero,Vector3.Zero};Ȅ=new MyDetectedEntityInfo(
69420,"No target.",MyDetectedEntityType.None,Vector3.Zero,MatrixD.Zero,Vector3.Zero,MyRelationsBetweenPlayerAndBlock.
NoOwnership,BoundingBoxD.CreateFromPoints(Ȝ()),0);Ǭ=new Dictionary<string,string>();ȅ=new Queue<string>();Me.CustomData="";if(debug
){ȅ.Enqueue("<< DEBUG: Debug switch TRUE! Begin debug output! >>\n");Me.CustomData+=
"<< DEBUG: Debug switch TRUE! Begin debug output! >>\n";}Runtime.UpdateFrequency=updateFrequency;if(debug){ȅ.Enqueue(
$"Setting update frequency to {(Runtime.UpdateFrequency)}\n");Me.CustomData+=$"Setting update frequency to {(Runtime.UpdateFrequency)}\n";}Ⱥ=new Ç();try{if(debug){ȅ.Enqueue(
"Attempting to activate WC API\n");Me.CustomData+=$"Attempting to activate WC API\n";}Ⱥ.ė(Me);if(debug){ȅ.Enqueue("WC API Active.\n");Me.CustomData+=
$"WC API Active.\n";}ǲ.Clear();try{Ⱥ.þ(Me,ǲ);}catch(Exception ex){if(debug){ȅ.Enqueue(
"Initial GetSortedThreats() failed. This may not be an issue.\n");Me.CustomData+=$"Initial GetSortedThreats() failed. This may not be an issue.\n";}}ȩ=ǲ.Count;if(debug){if(ȩ>0){ȅ.
Enqueue($"WC GotSortedThreats reports a count of {ȩ} threats!\n");Me.CustomData+=
$"WC GotSortedThreats reports a count of {ȩ} threats!\n";foreach(MyDetectedEntityInfo o in ǲ.Keys){ȅ.Enqueue(
$"Found target:\nName: {o.Name}\nVel: {o.Velocity}\nRelationship: {o.Relationship}\n");Me.CustomData+=$"Found target:\nName: {o.Name}\nVel: {o.Velocity}\nRelationship: {o.Relationship}\n";}}else{ȅ.Enqueue(
$"WC GotSortedThreats reports a count of zero!\n");Me.CustomData+=$"WC GotSortedThreats reports a count of zero!\n";}}}catch(Exception e){ȅ.Enqueue(
"WeaponCore API failed! \n");ȅ.Enqueue(e.StackTrace);Me.CustomData+="WeaponCore API failed! \n";Me.CustomData+=e.StackTrace;}try{if(debug){ȅ.
Enqueue("Attempting to activate DefenseShields API\n");Me.CustomData+="Attempting to activate DefenseShields API\n";}Ȼ=new s(Me
);if(debug){ȅ.Enqueue("DS API Active. Getting shield percentage...\n");Me.CustomData+=
"DS API Active. Getting shield percentage...\n";}ȧ=Ȼ.ź();if(debug){ȅ.Enqueue($"DS API Reports Shield Percentage: {ȧ} \n");Me.CustomData+=(
$"DS API Reports Shield Percentage: {ȧ} \n");}}catch(Exception e){ȅ.Enqueue("DefenseShields API activation failed!\n");ȅ.Enqueue(e.StackTrace);Me.CustomData+=(
"DefenseShields API activation failed!\n");Me.CustomData+=(e.StackTrace);throw e;}ǝ=new MySprite(){Type=SpriteType.TEXTURE,Data="Grid",Position=ǳ.Center,Size=ǳ.
Size*2,Color=Color.OrangeRed.Alpha(0.90f),Alignment=TextAlignment.CENTER,RotationOrScale=3/4f};Ǟ=new MySprite(){Type=
SpriteType.TEXTURE,Data="LCD_Emote_Sleepy",Position=ǳ.Center,Size=ǳ.Size/3,Alignment=TextAlignment.CENTER};ǟ=new MySprite{Type=
SpriteType.TEXT,Data="< -- MOORED -- >",Position=Ƿ,Size=ǳ.Size/3,RotationOrScale=0.9f,Color=Color.MediumPurple,Alignment=
TextAlignment.CENTER,FontId="White"};Ǡ=new MySprite(){Type=SpriteType.TEXTURE,Data="LCD_Emote_Happy",Position=ǳ.Center,Size=ǳ.Size/3,
Alignment=TextAlignment.CENTER};ǡ=new MySprite(){Type=SpriteType.TEXT,Data="<--STANDARD ALERT-->",Position=Ƿ,Size=ǳ.Size/3,
RotationOrScale=0.9f,Color=Color.MediumPurple,Alignment=TextAlignment.CENTER,FontId="White"};Ǣ=new MySprite(){Type=SpriteType.TEXTURE,
Data="LCD_Emote_Skeptical",Position=ǳ.Center,Size=ǳ.Size/3,Alignment=TextAlignment.CENTER};ǣ=new MySprite{Type=SpriteType.
TEXTURE,Data="LCD_Emote_Shocked",Position=ǳ.Center,Size=ǳ.Size/3,Alignment=TextAlignment.CENTER};Ǥ=new MySprite{Type=SpriteType
.TEXT,Data="<--YELLOW ALERT-->",Position=Ƿ,Size=ǳ.Size/3,RotationOrScale=0.9f,Color=Color.Yellow,Alignment=TextAlignment.
CENTER,FontId="Yellow"};ǥ=new MySprite(){Type=SpriteType.TEXTURE,Data="LCD_Emote_Angry",Position=ǳ.Center,Size=ǳ.Size/3,
Alignment=TextAlignment.CENTER};Ǧ=new MySprite{Type=SpriteType.TEXT,Data="<--RED ALERT-->",Position=Ƿ,Size=ǳ.Size/3,
RotationOrScale=0.9f,Color=Color.Red,Alignment=TextAlignment.CENTER,FontId="Red"};ǧ=new MySprite(){Type=SpriteType.TEXTURE,Data=
"Danger",Position=ǵ,Size=Ƿ,Alignment=TextAlignment.CENTER};Ǩ=new MySprite{Type=SpriteType.TEXT,Data="< -- LAST JUMP DRIVE -- >",
Position=ǳ.Center,Size=ǳ.Size/3,RotationOrScale=0.9f,Color=Color.Yellow,Alignment=TextAlignment.CENTER,FontId="Red"};ǩ=new
MySprite(){Type=SpriteType.TEXTURE,Data="Textures\\FactionLogo\\Builders\\BuilderIcon_5.dds",Position=Ƕ,Size=ǳ.Size/3,Color=
Color.IndianRed.Alpha(0.70f),Alignment=TextAlignment.CENTER};Ǫ=new MySprite{Type=SpriteType.TEXT,Data=ȧ.ToString(),Position=Ƿ
,Size=ǳ.Size/3,RotationOrScale=0.8f,Color=Color.SteelBlue,Alignment=TextAlignment.CENTER,FontId="White",};ǫ=new MySprite{
Type=SpriteType.TEXT,Data="Shield threshold triggered!\nGoing to Red Alert!\nRed Alert LOCKED until shields reach 90%!",
Position=Ƿ,Size=ǳ.Size/3,RotationOrScale=0.8f,Color=Color.SteelBlue,Alignment=TextAlignment.CENTER,FontId="White",};ǻ=new
MySprite{Type=SpriteType.TEXT,Data="OVERLORD SYSTEM LOADING...",Position=ǳ.Center,Size=ǳ.Size/3,RotationOrScale=0.5f,Color=Color
.Red,Alignment=TextAlignment.CENTER,FontId="Red"};ǭ=new MySprite(){Type=SpriteType.TEXT,Data=
"WEAPONCORE TARGETS IN-RANGE:",Position=ǳ.Center,Size=ǳ.Size/3,RotationOrScale=0.5f,Color=Color.Red,Alignment=TextAlignment.CENTER,FontId="Red"};Ȋ=new
MySprite(){Type=SpriteType.TEXT,Data=("No data!\n"),Position=ǳ.Center,Size=ǳ.Size/3,RotationOrScale=0.6f,Color=Color.Yellow,
Alignment=TextAlignment.CENTER,FontId="White"};Ȗ=new MySprite(){Type=SpriteType.TEXT,Data="<-- NO JUMP DRIVES! -->",Position=ǵ,
Size=ǳ.Size/3,RotationOrScale=0.9f,Color=Color.IndianRed,Alignment=TextAlignment.CENTER,FontId="Red"};Ǳ["redAlert"]=ȓ;Ǳ[
"yellowAlert"]=ʬ;Ǳ["standardAlert"]=ʮ;Ǳ["mooredAlert"]=ȥ;Ǳ["blueAlert"]=ʭ;Ǳ["save"]=Save;if(debug){ȅ.Enqueue(
"DEBUG: Populating variables via Block Groups\n");Me.CustomData+=("DEBUG: Populating variables via Block Groups\n");}try{ɣ=GridTerminalSystem.GetBlockGroupWithName(ȼ+
"IntAlertLights");if(ɣ!=null){ȅ.Enqueue("DEBUG: Found group IntAlertLights!\n");Me.CustomData+=("DEBUG: Found group IntAlertLights!\n");
}else{ȅ.Enqueue("DEBUG: Failed to find IntAlertLights!\n");Me.CustomData+=("DEBUG: Failed to find IntAlertLights!\n");}}
catch(Exception e){ȅ.Enqueue("DEBUG: Failed to get IntAlertLights!\n");ȅ.Enqueue(e.ToString());Me.CustomData+=(
"DEBUG: Failed to get IntAlertLights!\n");Me.CustomData+=(e.ToString());}try{ɤ=GridTerminalSystem.GetBlockGroupWithName(ȼ+"IntRotLights");if(ɥ!=null){ȅ.Enqueue(
"DEBUG: Found group IntRotLights!\n");Me.CustomData+=("DEBUG: Found group IntRotLights!\n");}else{ȅ.Enqueue("DEBUG: Failed to find IntRotLights!\n");Me.
CustomData+=("DEBUG: Failed to find IntRotLights!\n");}}catch(Exception e){ȅ.Enqueue("DEBUG: Failed to get IntRotLights!\n");ȅ.
Enqueue(e.ToString());Me.CustomData+=("DEBUG: Failed to get IntRotLights!\n");Me.CustomData+=(e.ToString());}try{ɦ=
GridTerminalSystem.GetBlockGroupWithName(ȼ+"ShipDoors");if(ɦ!=null){ȅ.Enqueue("DEBUG: Found group ShipDoors!\n");Me.CustomData+=(
"DEBUG: Found group ShipDoors!\n");}else{ȅ.Enqueue("DEBUG: Failed to find ShipDoors!\n");Me.CustomData+=("DEBUG: Failed to find ShipDoors!\n");}}catch(
Exception e){ȅ.Enqueue("DEBUG: Failed to get ShipDoors!\n");ȅ.Enqueue(e.ToString());Me.CustomData+=(
"DEBUG: Failed to get ShipDoors!\n");Me.CustomData+=(e.ToString());}try{ɥ=GridTerminalSystem.GetBlockGroupWithName(ȼ+"Spotlights");if(ɥ!=null){ȅ.Enqueue(
"DEBUG: Found group Spotlights!\n");Me.CustomData+=("DEBUG: Found group Spotlights!\n");}else{ȅ.Enqueue("DEBUG: Failed to find Spotlights!\n");Me.
CustomData+=("DEBUG: Failed to find Spotlights!\n");}}catch(Exception e){ȅ.Enqueue("DEBUG: Failed to get Spotlights!\n");ȅ.Enqueue
(e.ToString());Me.CustomData+=("DEBUG: Failed to get Spotlights!\n");Me.CustomData+=(e.ToString());}try{ɨ=
GridTerminalSystem.GetBlockGroupWithName(ȼ+"DCWelders");if(ɨ!=null){ȅ.Enqueue("DEBUG: Found group DCWelders!\n");Me.CustomData+=(
"DEBUG: Found group DCWelders!\n");}else{ȅ.Enqueue("DEBUG: Failed to find StatusLCD!\n");Me.CustomData+=("DEBUG: Failed to find StatusLCD!\n");}}catch(
Exception e){ȅ.Enqueue("DEBUG: Failed to get DCWelders!\n");ȅ.Enqueue(e.ToString());Me.CustomData+=(
"DEBUG: Failed to get DCWelders!\n");Me.CustomData+=(e.ToString());}try{ɩ=GridTerminalSystem.GetBlockGroupWithName(ȼ+"PMWWelders");if(ɩ!=null){ȅ.Enqueue(
"DEBUG: Found group PMWWelders!\n");Me.CustomData+=("DEBUG: Found group PMWWelders!\n");}else{ȅ.Enqueue("DEBUG: Failed to find PMWWelders!\n");Me.
CustomData+=("DEBUG: Failed to find PMWWelders!\n");}}catch(Exception e){ȅ.Enqueue("DEBUG: Failed to get PMWWelders!\n");ȅ.Enqueue
(e.ToString());Me.CustomData+=("DEBUG: Failed to get PMWWelders!\n");Me.CustomData+=(e.ToString());}try{ɪ=
GridTerminalSystem.GetBlockGroupWithName(ȼ+RailGunDoorsName);if(ɪ!=null){ȅ.Enqueue("DEBUG: Found group ShipGunDoorsGroup!\n");Me.
CustomData+=("DEBUG: Found group ShipGunDoorsGroup!\n");}else{ȅ.Enqueue("DEBUG: Failed to find ShipGunDoorsGroup!\n");Me.
CustomData+=("DEBUG: Failed to find ShipGunDoorsGroup!\n");}}catch(Exception e){ȅ.Enqueue("DEBUG: Failed to get GunDoors!\n");ȅ.
Enqueue(e.ToString());Me.CustomData+=("DEBUG: Failed to get GunDoors!\n");Me.CustomData+=(e.ToString());}try{ɬ=
GridTerminalSystem.GetBlockGroupWithName(ȼ+"IntSecSensors");if(ɬ!=null){ȅ.Enqueue("DEBUG: Found group IntSecSensors!\n");Me.CustomData+=(
"DEBUG: Found group IntSecSensors!\n");}else{ȅ.Enqueue("DEBUG: Failed to find IntSecSensors!\n");Me.CustomData+=("DEBUG: Failed to find IntSecSensors!\n");}}
catch(Exception e){ȅ.Enqueue("DEBUG: Failed to get IntSecSensors!\n");ȅ.Enqueue(e.ToString());Me.CustomData+=(
"DEBUG: Failed to get IntSecSensors!\n");Me.CustomData+=(e.ToString());}try{ɧ=GridTerminalSystem.GetBlockGroupWithName(ȼ+"StatusLCD");if(ɧ!=null){ȅ.Enqueue(
"DEBUG: Found group StatusLCDGroup!\n");Me.CustomData+=("DEBUG: Found group StatusLCDGroup!\n");}else{ȅ.Enqueue("DEBUG: Failed to find StatusLCDGroup!\n");Me.
CustomData+=("DEBUG: Failed to find StatusLCDGroup!\n");}}catch(Exception e){ȅ.Enqueue(
"DEBUG: Failed to get ShipStatusLCDGroup!\n");ȅ.Enqueue(e.ToString());Me.CustomData+=("DEBUG: Failed to get ShipStatusLCDGroup!\n");Me.CustomData+=(e.ToString());}
try{ɣ.GetBlocksOfType(ɳ);if(debug){Echo("DEBUG: Populated ShipAlertLighting via GroupName.\n");Me.CustomData+=(
"DEBUG: Populated ShipAlertLighting via GroupName.\n");}}catch(Exception e){Echo("Error populating ShipAlertLighting!\n");Me.CustomData+=(
"Error populating ShipAlertLighting!\n");}try{ɦ.GetBlocksOfType(ɴ);if(debug){Echo("DEBUG: Populated ShipDoorBlocks via GroupName.\n");Me.CustomData+=(
"DEBUG: Populated ShipDoorBlocks via GroupName.\n");}}catch(Exception e){Echo("Error populating ShipDoorBlocks!\n");Me.CustomData+=("Error populating ShipDoorBlocks!\n");
}try{ɨ.GetBlocksOfType(ɏ);if(debug){Echo("DEBUG: Populated ShipWelders via GroupName.\n");Me.CustomData+=(
"DEBUG: Populated ShipWelders via GroupName.\n");}}catch(Exception e){Echo("Error populating ShipWelders!\n");Me.CustomData+=("Error populating ShipWelders!\n");}try{ɪ
.GetBlocksOfType(ɘ);if(debug){Echo("DEBUG: Populated ShipGunDoorsBlocks via GroupName.\n");Me.CustomData+=(
"DEBUG: Populated ShipGunDoorsBlocks via GroupName.\n");}}catch(Exception e){Echo("Error populating ShipGunDoorsBlocks!\n");Me.CustomData+=(
"Error populating ShipGunDoorsBlocks!\n");}try{ɫ.GetBlocksOfType(ɇ);if(debug){Echo("DEBUG: Populated ShipSensorBlocks via GroupName.\n");Me.CustomData+=(
"DEBUG: Populated ShipSensorBlocks via GroupName.\n");}}catch(Exception e){Echo("Error populating ShipSensorBlocks!\n");Me.CustomData+=(
"Error populating ShipSensorBlocks!\n");}try{ɧ.GetBlocksOfType(ɔ);if(debug){Echo("DEBUG: Populated ShipStatusLCDBlocks via GroupName.\n");Me.CustomData+=(
"DEBUG: Populated ShipStatusLCDBlocks via GroupName.\n");}}catch(Exception e){Echo("Error populating ShipStatusLCDBlocks!\n");Me.CustomData+=(
"Error populating ShipStatusLCDBlocks!\n");}GridTerminalSystem.GetBlocksOfType(ɳ,m=>m.IsSameConstructAs(Me));GridTerminalSystem.GetBlocksOfType(ɴ,m=>m.
IsSameConstructAs(Me));GridTerminalSystem.GetBlocksOfType(ɶ,m=>m.IsSameConstructAs(Me));GridTerminalSystem.GetBlocksOfType(ɯ);
GridTerminalSystem.GetBlocksOfType(ɱ,m=>m.IsSameConstructAs(Me));GridTerminalSystem.GetBlocksOfType(ɲ,m=>m.IsSameConstructAs(Me));
GridTerminalSystem.GetBlocksOfType(ɷ,m=>m.IsSameConstructAs(Me));GridTerminalSystem.GetBlocksOfType(ɸ,m=>m.IsSameConstructAs(Me));
GridTerminalSystem.GetBlocksOfType(ɹ,m=>m.IsSameConstructAs(Me));GridTerminalSystem.GetBlocksOfType(ɺ,m=>m.IsSameConstructAs(Me));
GridTerminalSystem.GetBlocksOfType(ɼ,m=>m.IsSameConstructAs(Me));GridTerminalSystem.GetBlocksOfType(ɮ,m=>m.IsSameConstructAs(Me)&&(!m.
BlockDefinition.SubtypeName.ToLower().Contains("inhib")));GridTerminalSystem.GetBlocksOfType(ɡ,m=>m.IsSameConstructAs(Me));
GridTerminalSystem.GetBlocksOfType(ɒ,m=>m.IsSameConstructAs(Me));GridTerminalSystem.GetBlocksOfType(ɇ,m=>m.IsSameConstructAs(Me));
GridTerminalSystem.GetBlocksOfType(ɜ,m=>m.IsSameConstructAs(Me));GridTerminalSystem.GetBlocksOfType(Ɏ,m=>m.IsSameConstructAs(Me));
GridTerminalSystem.GetBlocksOfType(Ɉ,m=>m.IsSameConstructAs(Me));GridTerminalSystem.GetBlocksOfType(ɉ,m=>m.IsSameConstructAs(Me));
GridTerminalSystem.GetBlocksOfType(Ɋ,m=>m.IsSameConstructAs(Me));GridTerminalSystem.GetBlocksOfType(ɋ,m=>m.IsSameConstructAs(Me));
GridTerminalSystem.GetBlocksOfType(Ɍ,m=>m.IsSameConstructAs(Me));GridTerminalSystem.GetBlocksOfType(ɏ,m=>m.IsSameConstructAs(Me));
GridTerminalSystem.GetBlocksOfType(ɐ,m=>m.IsSameConstructAs(Me));GridTerminalSystem.GetBlocksOfType(ɟ,m=>m.IsSameConstructAs(Me));
GridTerminalSystem.GetBlocksOfType(ɓ,m=>m.IsSameConstructAs(Me));GridTerminalSystem.GetBlocksOfType(ɑ,m=>m.IsSameConstructAs(Me));
GridTerminalSystem.GetBlocksOfType(ɝ,m=>m.IsSameConstructAs(Me));GridTerminalSystem.GetBlocksOfType(ɞ,m=>m.IsSameConstructAs(Me));
GridTerminalSystem.GetBlocksOfType(ɠ,m=>m.IsSameConstructAs(Me));GridTerminalSystem.GetBlocksOfType(Ɇ,m=>m.IsSameConstructAs(Me));
GridTerminalSystem.GetBlocksOfType(Ȧ,m=>m.IsSameConstructAs(Me));GridTerminalSystem.GetBlocksOfType(ɍ,m=>m.IsSameConstructAs(Me));
GridTerminalSystem.GetBlocksOfType(ȋ,m=>m.IsSameConstructAs(Me));GridTerminalSystem.GetBlocksOfType(ə,ʐ=>ʐ.IsSameConstructAs(Me));
GridTerminalSystem.GetBlocksOfType(ɚ,ʐ=>ʐ.BlockDefinition.ToString().ToUpperInvariant().Contains("HYDRO")&ʐ.IsSameConstructAs(Me));
GridTerminalSystem.GetBlocksOfType(ɛ,ʐ=>ʐ.BlockDefinition.ToString().ToUpperInvariant().Contains("MODULAR")&ʐ.IsSameConstructAs(Me));if(
debug){Echo("Getting weaponcore turrets!\n");Me.CustomData+=("Getting weaponcore turrets!\n");}Ⱥ.Ć(ɭ);List<string>ê=new List<
string>();if(debug){foreach(string ʝ in ê){Echo($"DEBUG: WC found weapon with subtype name: {ʝ}\n");Me.CustomData+=(
$"DEBUG: WC found weapon with subtype name: {ʝ}\n");}}ɭ.ForEach(ã=>ê.Add(ã.SubtypeName));GridTerminalSystem.GetBlocksOfType<IMyTerminalBlock>(ɕ,m=>m.CubeGrid==Me.CubeGrid
&&ê.Contains(m.BlockDefinition.SubtypeName));GridTerminalSystem.GetBlocksOfType<IMyTerminalBlock>(ɖ,m=>m.CubeGrid==Me.
CubeGrid&&ê.Contains(m.BlockDefinition.SubtypeName)&&(m.BlockDefinition.SubtypeId.Contains("ARYXGaussCannon")||m.BlockDefinition
.SubtypeId.Contains("ARYXHeavyCoilgun")));GridTerminalSystem.GetBlocksOfType<IMyTerminalBlock>(ɗ,m=>m.CubeGrid==Me.
CubeGrid&&ê.Contains(m.BlockDefinition.SubtypeName)&&(m.BlockDefinition.SubtypeId.Contains("ARYXGaussCannon")||m.BlockDefinition
.SubtypeId.Contains("ARYXHeavyCoilgun")||m.BlockDefinition.SubtypeId.Contains("ARYXPlasmaPulser")||m.BlockDefinition.
SubtypeId.Contains("ARYXFocusLance")||m.BlockDefinition.SubtypeId.Contains("ARYXSmallPulseLaser_Fixed")));if(debug){Echo(
$"DEBUG: Number of ShipWeapons found: {ɕ.Count}\n");Echo($"DEBUG: Number of ShipRailguns found: {ɖ.Count}\n");Me.CustomData+=(
$"DEBUG: Number of ShipWeapons found: {ɕ.Count}\n");Me.CustomData+=($"DEBUG: Number of ShipRailguns found: {ɖ.Count}\n");}try{if(ɜ.Count>0){ɢ=ɜ[0].GetPosition();if(debug)
{Echo($"Getting ship world position!\nPosition:{ɢ.ToString()}\n");Me.CustomData+=(
$"Getting ship world position!\nPosition:{ɢ}\n");}}}catch(Exception ex){Echo("Exception setting shipWorldLocation. Is there a remote control?\n");Echo(ex.ToString());
Me.CustomData+=("Exception setting shipWorldLocation. Is there a remote control?\n");Me.CustomData+=(ex.ToString());}if(
debug){Echo("Setting Alert level to standardAlert.\n");Me.CustomData+=("Setting Alert level to standardAlert.\n");}Ʉ=Ȱ.Ȳ;Ɂ=Ȱ.
Ȳ;ʮ();foreach(IMyThrust ȍ in ɚ){ȍ.Enabled=false;ȍ.SetValueColor("FlameFullColorOverride",hydroThrusterColorFull);ȍ.
SetValueColor("FlameIdleColorOverride",hydroThrusterColorIdle);ȍ.ShowInTerminal=false;ȍ.ShowOnHUD=false;ȍ.ThrustOverride=0;}foreach(
IMyThrust Ȏ in ɛ){Ȏ.Enabled=true;Ȏ.SetValueColor("FlameFullColorOverride",ionThrusterColorFull);Ȏ.SetValueColor(
"FlameIdleColorOverride",ionThrusterColorIdle);Ȏ.ShowInTerminal=false;Ȏ.ShowOnHUD=false;Ȏ.ThrustOverride=0;}Ǖ=new string[2];ȇ=Storage.Split(';')
;Me.CustomData+="<-- Begin iterating storage string values -->\n";foreach(string ʟ in ȇ){Me.CustomData+=ʟ;try{Ǖ=ʟ.Split(
'=');}catch(Exception e){Echo(
$"DEBUG:Caught exception: StorageStringSplitTemp with value {ʟ.Split('=')} was liked the issue.\n");Me.CustomData+=($"DEBUG:Caught exception: StorageStringSplitTemp with value {ʟ.Split('=')} was liked the issue.\n");
continue;}if(Ǖ.Length==2){ǖ=Ǖ[0];Ǘ=Ǖ[1];Ǭ.Add(ǖ,Ǘ);if(debug){Echo(
$"DEBUG: Found StorageStringLabel {ǖ.ToString()} with value {Ǘ.ToString()}. Assigning to Dict {Ǭ.ToString()}\n");Me.CustomData+=(
$"DEBUG: Found StorageStringLabel {ǖ.ToString()} with value {Ǘ.ToString()}. Assigning to Dict {Ǭ.ToString()}\n");}}else{continue;}switch(ǖ){case"alertLevel":foreach(Ȱ ʠ in Enum.GetValues(typeof(Ȱ))){if(debug){Echo($"DEBUG: Iterating through AlertLevels to match against StorageString. Current: {((int)ʠ)} against stored {ʠ.ToString().Equals(Ǘ)}\n"
);Me.CustomData+=($"DEBUG: Iterating through AlertLevels to match against StorageString. Current: {((int)ʠ)} against stored {ʠ.ToString().Equals(Ǘ)}\n"
);}if(ʠ.ToString().Equals(Ǘ)){if(debug){Echo(
$"DEBUG: Found previously-stored value of: {(ʠ)} in StorageString! -> Applying to ShipAlertLevel!\n");Me.CustomData+=($"DEBUG: Found previously-stored value of: {(ʠ)} in StorageString! -> Applying to ShipAlertLevel!\n");
}Ɂ=ʠ;Ʉ=ʠ;}}break;default:break;}Me.CustomData+="Processed the following Saved string:\n"+ʟ;}Me.CustomData+=
"<-- End iterating storage string values -->\n";Storage=ȉ;try{ȹ=new ǂ(Ⱥ,ɖ,ǲ,Me,GridTerminalSystem,ȼ+GyroGroupName,ȼ+GunGroupName);}catch(Exception ex){Me.CustomData+=
$"Aimbot failed to start! Exception below:\n<--------->\n";Me.CustomData+=ex.ToString();Me.CustomData+=$"\n<--------->\n";}Me.CustomData+=
$"Aimbot reports:\n Initialized - {ȹ.ƴ}\n Has gun group - {ȹ.ƨ!=null}\n";Me.CustomData+="Buckle up, buttercup. OVERLORD loop has started!\n";}void Save(){Echo(">> OVERLORD Saving! >>\n");Me.
CustomData+=(">> OVERLORD Saving! >>\n");if(debug){ȅ.Enqueue($"DEBUG: Save() StorageString at time of store is: {Ȯ}\n");}Storage=Ȯ
.ToString();}void Main(string Ļ,UpdateType ʡ){if(ʡ==UpdateType.Terminal||ʡ==UpdateType.None||(ʡ==UpdateType.Trigger&&Ʌ.
TryParse(Ļ))){debug=Ʌ.Switch("debug");Ȇ=Ʌ.Argument(0);if(debug){ȅ.Enqueue(
"<< DEBUG:Runtime Debug switch TRUE! Begin debug output! >> \n");Me.CustomData+=("<< DEBUG:Runtime Debug switch TRUE! Begin debug output! >> \n");}Ǯ=Ʌ.Switch("base");ǔ=Ʌ.Switch(
"miner");if(Ȇ==null){ȅ.Enqueue("WARNING: No command specified!\n");}else if(Ǳ.TryGetValue(Ȇ,out Ƀ)&&Ƀ!=null){if(debug){ȅ.
Enqueue($"DEBUG: Found command {Ƀ}\n");Me.CustomData+=($"DEBUG: Found command {Ƀ}\n");}Ƀ();}}if(Ȇ!=null&&ȹ.ƴ&&(Ȇ.Equals(
"toggle"))){if(ȹ!=null){if(!ȹ.Ƴ){Runtime.UpdateFrequency=UpdateFrequency.Update1;}else{Runtime.UpdateFrequency=updateFrequency;}
ȹ.ĺ(Ļ,Me);}}if(ʡ==UpdateType.Update10||ʡ==UpdateType.Update100){Echo(">> OVERLORD >>\n");ǲ.Clear();Ⱥ.þ(Me,ǲ);ȩ=ǲ.Count;if
(ǲ.Count<0){ȩ=0;}ȧ=Ȼ.ź();if(ȧ<0){ȧ=0;}if(debug){Me.CustomData+=(">> OVERLORD Runtime stats >>\n");Me.CustomData+=(
$"Current Alert Level is:{Ț(Ʉ)}\n");Me.CustomData+=($"Previous Alert Level was:{Ț(Ɂ)}\n");Me.CustomData+=($"RedAlertTriggeredByEvent: {ȿ}\n");Me.
CustomData+=($"DefenseShields percentage: {ȧ}\n");Me.CustomData+=($"WCThreats List Size: {ǲ.Count}\n");Me.CustomData+=(
$"WCThreatCount value: {ȩ}\n");Me.CustomData+=($"Time since last run is:{Runtime.TimeSinceLastRun}\n");Me.CustomData+=(
$"Last runtime (Ms) was: {Runtime.LastRunTimeMs}\n");Me.CustomData+=($"Current instruction count is:{Runtime.CurrentInstructionCount}\n");Me.CustomData+=(
$"Max instructions count is:{Runtime.MaxInstructionCount}\n");Me.CustomData+=($"Current Callchain Depth is:{Runtime.CurrentCallChainDepth}\n");Me.CustomData+=(
$"Max callchain depth is:{Runtime.MaxCallChainDepth}\n");foreach(string ʟ in ȇ){Me.CustomData+=($"DEBUG: Found the following key/value pair in storage string: \n{ʟ}\n");}}else
{ȅ.Enqueue("OVERLORD >> Runtime stats:\n");ȅ.Enqueue($"Ship Alert Status is: {Ț(Ʉ)}\n");ȅ.Enqueue(
$"Time since last run is:{Runtime.TimeSinceLastRun}\n");ȅ.Enqueue($"Last runtime (Ms) was: {Runtime.LastRunTimeMs}\n");ȅ.Enqueue($"Current Alert Level is:{Ț(Ʉ)}\n");ȅ.Enqueue
($"Previous Alert Level was:{Ț(Ɂ)}\n");ȅ.Enqueue($"Current instruction count is:{Runtime.CurrentInstructionCount}\n");ȅ.
Enqueue($"Max instructions count is:{Runtime.MaxInstructionCount}\n");ȅ.Enqueue(
$"Current Callchain Depth is:{Runtime.CurrentCallChainDepth}\n");ȅ.Enqueue($"Max callchain depth is:{Runtime.MaxCallChainDepth}\n");ȅ.Enqueue($"Ship under player control?: {Ȩ}\n");}
switch(Ʉ){case Ȱ.Ȳ:ȝ(debug).MoveNext();break;case Ȱ.ȳ:ȡ(debug).MoveNext();break;case Ȱ.ȴ:Ȣ(debug).MoveNext();break;case Ȱ.ȷ:ȣ(
debug).MoveNext();break;case Ȱ.ȱ:Ȥ(debug).MoveNext();break;default:ȅ.Enqueue(
"ShipAlertLevel switch hit default! No actions taken!\n");break;}Echo($"<---LAST LOG ENTRY AS OF {DateTime.Now}--->:\n");if(ȅ.Count>0){if(debug){Echo(
$"DEBUG: logTimeAccum is:{ɀ},logSpeed: {ɂ}");}if(ɀ<=ɂ){ɀ+=((int)Runtime.TimeSinceLastRun.TotalSeconds);Echo(Ƚ);}else{Ƚ=ȅ.Dequeue();Me.CustomData+=Ƚ;ɀ=0;}}for(int Į
=0;Į<Me.SurfaceCount;Į++){ȯ=Me.GetSurface(Į);switch(Ʉ){case Ȱ.Ȳ:ʶ(ȯ);break;case Ȱ.ȳ:ʷ(ȯ);break;case Ȱ.ȴ:ʸ(ȯ);break;case Ȱ
.ȷ:break;case Ȱ.ȱ:ʵ(ȯ);break;default:ʶ(ȯ);break;}}}}List<IMyTerminalBlock>è=new List<IMyTerminalBlock>();List<
MyDefinitionId>é=new List<MyDefinitionId>();List<string>ê=new List<string>();Dictionary<String,int>ʞ=new Dictionary<String,int>();
Dictionary<MyDetectedEntityInfo,float>ʜ=new Dictionary<MyDetectedEntityInfo,float>();void ʖ(bool ʌ){Echo(
"Printing Ion and Hydrogen Thrusters!\n");List<IMyThrust>ʍ=new List<IMyThrust>();List<IMyThrust>ʎ=new List<IMyThrust>();List<IMyThrust>ʏ=new List<IMyThrust>();
GridTerminalSystem.GetBlocksOfType(ʍ);if(ʍ.Count==0){if(ʌ){Echo("No Thrusters populated in list!\n");}return;}else{foreach(IMyThrust ʐ in
ʍ){if(ʌ){Echo($"Found Thruster:{ʐ.BlockDefinition.SubtypeName}");}if(ʐ.BlockDefinition.ToString().ToUpperInvariant().
Contains("HYDRO")){ʏ.Add(ʐ);}if(ʐ.BlockDefinition.ToString().ToUpperInvariant().Contains("MODULAR")){ʎ.Add(ʐ);}}Echo(
$"Found {ʏ.Count()} hydro thrusters and {ʎ.Count()} ion thrusters!\n");foreach(IMyThrust ʑ in ɛ){Echo($"Found Ion Thruster {ʑ.BlockDefinition.SubtypeId}");}foreach(IMyThrust ȍ in ʏ){Echo(
$"Found Hydro Thruster {ȍ.BlockDefinition.SubtypeId}");}}}void ʒ(IMyTextSurface ʓ){IMyCubeGrid ʔ=Me.CubeGrid;List<IMySlimBlock>ʕ=new List<IMySlimBlock>();MySprite ʗ=new
MySprite(){Alignment=TextAlignment.CENTER,};GridTerminalSystem.GetBlocksOfType<IMySlimBlock>(ʕ);for(int Į=0;Į<ʕ.Count;Į++){Echo(
ʕ[Į].Position.ToString());}}void ʘ(){List<string>ð=new List<string>();Me.GetSurface(0).GetScripts(ð);Echo(
"Printing available scripts");foreach(string ñ in ð){Echo(ñ);}}void ʙ(){Ⱥ.Ć(ɭ);List<string>ê=new List<string>();ɭ.ForEach(ã=>ê.Add(ã.SubtypeName));
GridTerminalSystem.GetBlocksOfType<IMyTerminalBlock>(ɕ,m=>m.CubeGrid==Me.CubeGrid&&ê.Contains(m.BlockDefinition.SubtypeName));Echo(
"WC Found the following weapons:\n");foreach(IMyTerminalBlock A in ɕ){Echo($"{A.BlockDefinition.SubtypeName}");}}void ʚ(){List<IMyTerminalBlock>è=new List<
IMyTerminalBlock>();List<MyDefinitionId>é=new List<MyDefinitionId>();Dictionary<string,int>ʛ=new Dictionary<string,int>();Ⱥ.ù(é);List<
string>ê=new List<string>();é.ForEach(ã=>ê.Add(ã.SubtypeName));GridTerminalSystem.GetBlocksOfType<IMyTerminalBlock>(è,v=>v.
CubeGrid==Me.CubeGrid&&ê.Contains(v.BlockDefinition.SubtypeName));Echo("Printing Weaponcore Turrets\n");foreach(IMyTerminalBlock
ä in è){Ⱥ.ú(ä,ʛ);foreach(string å in ʛ.Keys){Echo(å.ToString());Echo(ʛ[å].ToString());}}}void æ(Ç ç){List<
IMyTerminalBlock>è=new List<IMyTerminalBlock>();List<MyDefinitionId>é=new List<MyDefinitionId>();List<string>ê=new List<string>();ç.ù(é)
;é.ForEach(ã=>ê.Add(ã.SubtypeName));GridTerminalSystem.GetBlocksOfType<IMyTerminalBlock>(è,v=>v.CubeGrid==Me.CubeGrid&&ê.
Contains(v.BlockDefinition.SubtypeName));Echo("Firing Weaponcore Turrets\n");foreach(IMyTerminalBlock ä in è){ç.U(ä,true,true,0)
;}}void ë(IMyTerminalBlock m){List<ITerminalProperty>ô=new List<ITerminalProperty>();m.GetProperties(ô);foreach(var í in
ô){Echo(í.Id);}}void î(IMyProgrammableBlock ï){List<string>ð=new List<string>();ï.GetSurface(0).GetScripts(ð);Echo(
"Printing available scripts");foreach(string ñ in ð){Echo(ñ);}}void ò(Ç ç,IMyTerminalBlock ó){ç.ù(é);List<string>ê=new List<string>();é.ForEach(ã=>ê
.Add(ã.SubtypeName));GridTerminalSystem.GetBlocksOfType<IMyTerminalBlock>(è,v=>v.CubeGrid==Me.CubeGrid&&ê.Contains(v.
BlockDefinition.SubtypeName));foreach(IMyTerminalBlock ä in è){ç.ú(ä,ʞ);}ç.þ(ó,ʜ);Echo("Printing Sorted Threats.\n");foreach(
MyDetectedEntityInfo â in ʜ.Keys){Echo(â.Name.ToString());Echo(â.Position.ToString());Echo(â.Velocity.ToString());}}
}public class Ç{private Action<ICollection<MyDefinitionId>>à;private Action<ICollection<MyDefinitionId>>È;private Action<
ICollection<MyDefinitionId>>É;private Func<Sandbox.ModAPI.Ingame.IMyTerminalBlock,IDictionary<string,int>,bool>Ê;private Func<long,
MyTuple<bool,int,int>>Ë;private Action<Sandbox.ModAPI.Ingame.IMyTerminalBlock,IDictionary<MyDetectedEntityInfo,float>>Ì;private
Action<Sandbox.ModAPI.Ingame.IMyTerminalBlock,ICollection<Sandbox.ModAPI.Ingame.MyDetectedEntityInfo>>Í;private Func<long,int,
MyDetectedEntityInfo>Î;private Func<Sandbox.ModAPI.Ingame.IMyTerminalBlock,long,int,bool>Ï;private Func<Sandbox.ModAPI.Ingame.
IMyTerminalBlock,long,bool>Ð;private Func<Sandbox.ModAPI.Ingame.IMyTerminalBlock,int,MyDetectedEntityInfo>Ñ;private Action<Sandbox.
ModAPI.Ingame.IMyTerminalBlock,long,int>Ò;private Action<Sandbox.ModAPI.Ingame.IMyTerminalBlock,bool,int>Ó;private Action<
Sandbox.ModAPI.Ingame.IMyTerminalBlock,bool,bool,int>Ô;private Func<Sandbox.ModAPI.Ingame.IMyTerminalBlock,int,bool,bool,bool>Õ
;private Func<Sandbox.ModAPI.Ingame.IMyTerminalBlock,int,float>Ö;private Func<Sandbox.ModAPI.Ingame.IMyTerminalBlock,
ICollection<string>,int,bool>Ø;private Action<Sandbox.ModAPI.Ingame.IMyTerminalBlock,ICollection<string>,int>Ù;private Action<
Sandbox.ModAPI.Ingame.IMyTerminalBlock,float>Ú;private Func<Sandbox.ModAPI.Ingame.IMyTerminalBlock,long,int,bool>Û;private Func
<Sandbox.ModAPI.Ingame.IMyTerminalBlock,long,int,MyTuple<bool,Vector3D?>>Ü;private Func<Sandbox.ModAPI.Ingame.
IMyTerminalBlock,long,int,bool>Ý;private Func<Sandbox.ModAPI.Ingame.IMyTerminalBlock,long,int,Vector3D?>Þ;private Func<Sandbox.ModAPI.
Ingame.IMyTerminalBlock,float>ß;private Func<Sandbox.ModAPI.Ingame.IMyTerminalBlock,float>ì;private Func<MyDefinitionId,float>
á;private Func<long,bool>õ;private Func<Sandbox.ModAPI.Ingame.IMyTerminalBlock,bool>ć;private Func<long,float>Ĉ;private
Func<Sandbox.ModAPI.Ingame.IMyTerminalBlock,int,string>ĉ;private Action<Sandbox.ModAPI.Ingame.IMyTerminalBlock,int,string>Ċ;
private Action<Sandbox.ModAPI.Ingame.IMyTerminalBlock,int,Action<long,int,ulong,long,Vector3D,bool>>ċ;private Action<Sandbox.
ModAPI.Ingame.IMyTerminalBlock,int,Action<long,int,ulong,long,Vector3D,bool>>Č;private Func<ulong,MyTuple<Vector3D,Vector3D,
float,float,long,string>>č;private Func<long,float>Ď;private Func<Sandbox.ModAPI.Ingame.IMyTerminalBlock,long>ď;private Func<
Sandbox.ModAPI.Ingame.IMyTerminalBlock,int,Matrix>Đ;private Func<Sandbox.ModAPI.Ingame.IMyTerminalBlock,int,Matrix>đ;private
Func<Sandbox.ModAPI.Ingame.IMyTerminalBlock,long,bool,bool,bool>Ē;private Func<Sandbox.ModAPI.Ingame.IMyTerminalBlock,int,
MyTuple<Vector3D,Vector3D>>Ĕ;private Func<Sandbox.ModAPI.Ingame.IMyTerminalBlock,MyTuple<bool,bool>>ğ;private Action<Sandbox.
ModAPI.Ingame.IMyTerminalBlock,int,Action<int,bool>>ĕ;private Action<Sandbox.ModAPI.Ingame.IMyTerminalBlock,int,Action<int,
bool>>Ė;public bool ė(Sandbox.ModAPI.Ingame.IMyTerminalBlock Ę){var ę=Ę.GetProperty("WcPbAPI")?.As<IReadOnlyDictionary<
string,Delegate>>().GetValue(Ę);if(ę==null)throw new Exception("WcPbAPI failed to activate");return Ě(ę);}public bool Ě(
IReadOnlyDictionary<string,Delegate>ě){if(ě==null)return false;Ĝ(ě,"GetCoreWeapons",ref à);Ĝ(ě,"GetCoreStaticLaunchers",ref È);Ĝ(ě,
"GetCoreTurrets",ref É);Ĝ(ě,"GetBlockWeaponMap",ref Ê);Ĝ(ě,"GetProjectilesLockedOn",ref Ë);Ĝ(ě,"GetSortedThreats",ref Ì);Ĝ(ě,
"GetObstructions",ref Í);Ĝ(ě,"GetAiFocus",ref Î);Ĝ(ě,"SetAiFocus",ref Ï);Ĝ(ě,"ReleaseAiFocus",ref Ð);Ĝ(ě,"GetWeaponTarget",ref Ñ);Ĝ(ě,
"SetWeaponTarget",ref Ò);Ĝ(ě,"FireWeaponOnce",ref Ó);Ĝ(ě,"ToggleWeaponFire",ref Ô);Ĝ(ě,"IsWeaponReadyToFire",ref Õ);Ĝ(ě,
"GetMaxWeaponRange",ref Ö);Ĝ(ě,"GetTurretTargetTypes",ref Ø);Ĝ(ě,"SetTurretTargetTypes",ref Ù);Ĝ(ě,"SetBlockTrackingRange",ref Ú);Ĝ(ě,
"IsTargetAligned",ref Û);Ĝ(ě,"IsTargetAlignedExtended",ref Ü);Ĝ(ě,"CanShootTarget",ref Ý);Ĝ(ě,"GetPredictedTargetPosition",ref Þ);Ĝ(ě,
"GetHeatLevel",ref ß);Ĝ(ě,"GetCurrentPower",ref ì);Ĝ(ě,"GetMaxPower",ref á);Ĝ(ě,"HasGridAi",ref õ);Ĝ(ě,"HasCoreWeapon",ref ć);Ĝ(ě,
"GetOptimalDps",ref Ĉ);Ĝ(ě,"GetActiveAmmo",ref ĉ);Ĝ(ě,"SetActiveAmmo",ref Ċ);Ĝ(ě,"MonitorProjectile",ref ċ);Ĝ(ě,"UnMonitorProjectile",
ref Č);Ĝ(ě,"GetProjectileState",ref č);Ĝ(ě,"GetConstructEffectiveDps",ref Ď);Ĝ(ě,"GetPlayerController",ref ď);Ĝ(ě,
"GetWeaponAzimuthMatrix",ref Đ);Ĝ(ě,"GetWeaponElevationMatrix",ref đ);Ĝ(ě,"IsTargetValid",ref Ē);Ĝ(ě,"GetWeaponScope",ref Ĕ);Ĝ(ě,"IsInRange",ref
ğ);Ĝ(ě,"RegisterEventMonitor",ref ĕ);Ĝ(ě,"UnRegisterEventMonitor",ref Ė);return true;}private void Ĝ<ĝ>(
IReadOnlyDictionary<string,Delegate>ě,string Ğ,ref ĝ Ġ)where ĝ:class{if(ě==null){Ġ=null;return;}Delegate ē;if(!ě.TryGetValue(Ğ,out ē))throw
new Exception($"{GetType().Name} :: Couldn't find {Ğ} delegate of type {typeof(ĝ)}");Ġ=ē as ĝ;if(Ġ==null)throw new
Exception($"{GetType().Name} :: Delegate {Ğ} is not type {typeof(ĝ)}, instead it's: {ē.GetType()}");}public void Ć(ICollection<
MyDefinitionId>L)=>à?.Invoke(L);public void ø(ICollection<MyDefinitionId>L)=>È?.Invoke(L);public void ù(ICollection<MyDefinitionId>L)
=>É?.Invoke(L);public bool ú(Sandbox.ModAPI.Ingame.IMyTerminalBlock û,IDictionary<string,int>L)=>Ê?.Invoke(û,L)??false;
public MyTuple<bool,int,int>ü(long ý)=>Ë?.Invoke(ý)??new MyTuple<bool,int,int>();public void þ(Sandbox.ModAPI.Ingame.
IMyTerminalBlock ó,IDictionary<MyDetectedEntityInfo,float>L)=>Ì?.Invoke(ó,L);public void Ą(Sandbox.ModAPI.Ingame.IMyTerminalBlock ó,
ICollection<Sandbox.ModAPI.Ingame.MyDetectedEntityInfo>L)=>Í?.Invoke(ó,L);public MyDetectedEntityInfo?ÿ(long Ā,int ā=0)=>Î?.Invoke(
Ā,ā);public bool Ă(Sandbox.ModAPI.Ingame.IMyTerminalBlock ó,long Q,int ā=0)=>Ï?.Invoke(ó,Q,ā)??false;public bool ă(
Sandbox.ModAPI.Ingame.IMyTerminalBlock ó,long ą)=>Ð?.Invoke(ó,ą)??false;public MyDetectedEntityInfo?ö(Sandbox.ModAPI.Ingame.
IMyTerminalBlock A,int B=0)=>Ñ?.Invoke(A,B);public void P(Sandbox.ModAPI.Ingame.IMyTerminalBlock A,long Q,int B=0)=>Ò?.Invoke(A,Q,B);
public void R(Sandbox.ModAPI.Ingame.IMyTerminalBlock A,bool S=true,int B=0)=>Ó?.Invoke(A,S,B);public void U(Sandbox.ModAPI.
Ingame.IMyTerminalBlock A,bool V,bool S,int B=0)=>Ô?.Invoke(A,V,S,B);public bool W(Sandbox.ModAPI.Ingame.IMyTerminalBlock A,
int B=0,bool X=true,bool Y=false)=>Õ?.Invoke(A,B,X,Y)??false;public float Z(Sandbox.ModAPI.Ingame.IMyTerminalBlock A,int B)
=>Ö?.Invoke(A,B)??0f;public bool a(Sandbox.ModAPI.Ingame.IMyTerminalBlock A,IList<string>L,int B=0)=>Ø?.Invoke(A,L,B)??
false;public void N(Sandbox.ModAPI.Ingame.IMyTerminalBlock A,IList<string>L,int B=0)=>Ù?.Invoke(A,L,B);public void C(Sandbox.
ModAPI.Ingame.IMyTerminalBlock A,float D)=>Ú?.Invoke(A,D);public bool E(Sandbox.ModAPI.Ingame.IMyTerminalBlock A,long F,int B)
=>Û?.Invoke(A,F,B)??false;public MyTuple<bool,Vector3D?>G(Sandbox.ModAPI.Ingame.IMyTerminalBlock A,long F,int B)=>Ü?.
Invoke(A,F,B)??new MyTuple<bool,Vector3D?>();public bool H(Sandbox.ModAPI.Ingame.IMyTerminalBlock A,long F,int B)=>Ý?.Invoke(A
,F,B)??false;public Vector3D?I(Sandbox.ModAPI.Ingame.IMyTerminalBlock A,long F,int B)=>Þ?.Invoke(A,F,B)??null;public
float J(Sandbox.ModAPI.Ingame.IMyTerminalBlock A)=>ß?.Invoke(A)??0f;public float K(Sandbox.ModAPI.Ingame.IMyTerminalBlock A)
=>ì?.Invoke(A)??0f;public float M(MyDefinitionId c)=>á?.Invoke(c)??0f;public bool y(long o)=>õ?.Invoke(o)??false;public
bool z(Sandbox.ModAPI.Ingame.IMyTerminalBlock A)=>ć?.Invoke(A)??false;public float ª(long o)=>Ĉ?.Invoke(o)??0f;public string
µ(Sandbox.ModAPI.Ingame.IMyTerminalBlock A,int B)=>ĉ?.Invoke(A,B)??null;public void º(Sandbox.ModAPI.Ingame.
IMyTerminalBlock A,int B,string À)=>Ċ?.Invoke(A,B,À);public void Å(Sandbox.ModAPI.Ingame.IMyTerminalBlock A,int B,Action<long,int,ulong,
long,Vector3D,bool>q)=>ċ?.Invoke(A,B,q);public void Á(Sandbox.ModAPI.Ingame.IMyTerminalBlock A,int B,Action<long,int,ulong,
long,Vector3D,bool>q)=>Č?.Invoke(A,B,q);public MyTuple<Vector3D,Vector3D,float,float,long,string>Â(ulong Ã)=>č?.Invoke(Ã)??
new MyTuple<Vector3D,Vector3D,float,float,long,string>();public float Ä(long o)=>Ď?.Invoke(o)??0f;public long Æ(Sandbox.
ModAPI.Ingame.IMyTerminalBlock A)=>ď?.Invoke(A)??-1;public Matrix w(Sandbox.ModAPI.Ingame.IMyTerminalBlock A,int B)=>Đ?.Invoke
(A,B)??Matrix.Zero;public Matrix e(Sandbox.ModAPI.Ingame.IMyTerminalBlock A,int B)=>đ?.Invoke(A,B)??Matrix.Zero;public
bool f(Sandbox.ModAPI.Ingame.IMyTerminalBlock A,long g,bool h,bool k)=>Ē?.Invoke(A,g,h,k)??false;public MyTuple<Vector3D,
Vector3D>l(Sandbox.ModAPI.Ingame.IMyTerminalBlock A,int B)=>Ĕ?.Invoke(A,B)??new MyTuple<Vector3D,Vector3D>();public MyTuple<bool
,bool>u(Sandbox.ModAPI.Ingame.IMyTerminalBlock m)=>ğ?.Invoke(m)??new MyTuple<bool,bool>();public void n(Sandbox.ModAPI.
Ingame.IMyTerminalBlock o,int p,Action<int,bool>q)=>ĕ?.Invoke(o,p,q);public void r(Sandbox.ModAPI.Ingame.IMyTerminalBlock o,
int p,Action<int,bool>q)=>Ė?.Invoke(o,p,q);}class s{private IMyTerminalBlock t;private Func<IMyTerminalBlock,RayD,Vector3D?
>O;private Func<IMyTerminalBlock,LineD,Vector3D?>ġ;private Func<IMyTerminalBlock,Vector3D,bool>Ķ;private Func<
IMyTerminalBlock,float>Ǝ;private Func<IMyTerminalBlock,int>Ə;private Func<IMyTerminalBlock,float>Ɛ;private Func<IMyTerminalBlock,int>Ƒ;
private Func<IMyTerminalBlock,float>ƒ;private Func<IMyTerminalBlock,float>Ɠ;private Func<IMyTerminalBlock,float>Ɣ;private Func<
IMyTerminalBlock,float>ƕ;private Func<IMyTerminalBlock,float>Ɩ;private Func<IMyTerminalBlock,bool>Ɨ;private Func<IMyTerminalBlock,string
>Ƙ;private Func<IMyTerminalBlock,IMyEntity,bool,bool>ƙ;private Func<IMyTerminalBlock,long,bool,bool>ƚ;private Func<
IMyCubeGrid,bool>ƣ;private Func<IMyCubeGrid,bool>ƛ;private Func<IMyEntity,bool>Ɯ;private Func<IMyEntity,IMyTerminalBlock>Ɲ;private
Func<IMyTerminalBlock,bool>ƞ;private Func<Vector3D,IMyTerminalBlock>Ɵ;private Func<IMyTerminalBlock,Vector3D,double>Ơ;
private Func<IMyTerminalBlock,Vector3D,Vector3D?>ơ;public void Ƣ(IMyTerminalBlock m)=>t=m;public s(IMyTerminalBlock m){t=m;var
ě=t.GetProperty("DefenseSystemsPbAPI")?.As<IReadOnlyDictionary<string,Delegate>>().GetValue(t);if(ě==null)return;O=(Func<
IMyTerminalBlock,RayD,Vector3D?>)ě["RayIntersectShield"];ġ=(Func<IMyTerminalBlock,LineD,Vector3D?>)ě["LineIntersectShield"];Ķ=(Func<
IMyTerminalBlock,Vector3D,bool>)ě["PointInShield"];Ǝ=(Func<IMyTerminalBlock,float>)ě["GetShieldPercent"];Ə=(Func<IMyTerminalBlock,int>)ě
["GetShieldHeat"];Ɛ=(Func<IMyTerminalBlock,float>)ě["GetChargeRate"];Ƒ=(Func<IMyTerminalBlock,int>)ě["HpToChargeRatio"];ƒ
=(Func<IMyTerminalBlock,float>)ě["GetMaxCharge"];Ɠ=(Func<IMyTerminalBlock,float>)ě["GetCharge"];Ɣ=(Func<IMyTerminalBlock,
float>)ě["GetPowerUsed"];ƕ=(Func<IMyTerminalBlock,float>)ě["GetPowerCap"];Ɩ=(Func<IMyTerminalBlock,float>)ě["GetMaxHpCap"];Ɨ=
(Func<IMyTerminalBlock,bool>)ě["IsShieldUp"];Ƙ=(Func<IMyTerminalBlock,string>)ě["ShieldStatus"];ƙ=(Func<IMyTerminalBlock,
IMyEntity,bool,bool>)ě["EntityBypass"];ƚ=(Func<IMyTerminalBlock,long,bool,bool>)ě["EntityBypassPb"];ƣ=(Func<IMyCubeGrid,bool>)ě[
"GridHasShield"];ƛ=(Func<IMyCubeGrid,bool>)ě["GridShieldOnline"];Ɯ=(Func<IMyEntity,bool>)ě["ProtectedByShield"];Ɲ=(Func<IMyEntity,
IMyTerminalBlock>)ě["GetShieldBlock"];ƞ=(Func<IMyTerminalBlock,bool>)ě["IsShieldBlock"];Ɵ=(Func<Vector3D,IMyTerminalBlock>)ě[
"GetClosestShield"];Ơ=(Func<IMyTerminalBlock,Vector3D,double>)ě["GetDistanceToShield"];ơ=(Func<IMyTerminalBlock,Vector3D,Vector3D?>)ě[
"GetClosestShieldPoint"];if(!ƾ())t=ƽ(t.CubeGrid)??t;}public Vector3D?ƍ(RayD Ŷ)=>O?.Invoke(t,Ŷ)??null;public Vector3D?Ƌ(LineD ŷ)=>ġ?.Invoke(t,ŷ)
??null;public bool Ÿ(Vector3D Ź)=>Ķ?.Invoke(t,Ź)??false;public float ź()=>Ǝ?.Invoke(t)??-1;public int Ż()=>Ə?.Invoke(t)??-
1;public float ż()=>Ɛ?.Invoke(t)??-1;public float Ž()=>Ƒ?.Invoke(t)??-1;public float ž()=>ƒ?.Invoke(t)??-1;public float ſ
()=>Ɠ?.Invoke(t)??-1;public float ƀ()=>Ɣ?.Invoke(t)??-1;public float Ɓ()=>ƕ?.Invoke(t)??-1;public float Ƃ()=>Ɩ?.Invoke(t)
??-1;public bool ƃ()=>Ɨ?.Invoke(t)??false;public string Ƅ()=>Ƙ?.Invoke(t)??string.Empty;public bool ƅ(IMyEntity o,bool Ɔ=
false)=>ƙ?.Invoke(t,o,Ɔ)??false;public bool Ƈ(long o,bool Ɔ=false)=>ƚ?.Invoke(t,o,Ɔ)??false;public bool ƈ(IMyCubeGrid Ɖ)=>ƣ?.
Invoke(Ɖ)??false;public bool Ɗ(IMyCubeGrid Ɖ)=>ƛ?.Invoke(Ɖ)??false;public bool ƌ(IMyEntity o)=>Ɯ?.Invoke(o)??false;public
IMyTerminalBlock ƽ(IMyEntity o)=>Ɲ?.Invoke(o)??null;public bool ƾ()=>ƞ?.Invoke(t)??false;public IMyTerminalBlock ƿ(Vector3D Ź)=>Ɵ?.
Invoke(Ź)??null;public double ǀ(Vector3D Ź)=>Ơ?.Invoke(t,Ź)??-1;public Vector3D?ǁ(Vector3D Ź)=>ơ?.Invoke(t,Ź)??null;}class ǂ{
string ǃ="guns";string Ǆ="gyros";double ǆ=0.1;const double ǒ=40;const double Ǉ=15;const double ǈ=13;const double ǉ=15;const
double Ǌ=0;const double ǋ=7;double ǌ=0;double Ǎ=0;double ǎ=0;double Ǐ=3.14;double ǐ=Math.PI/180*5;double Ǒ=0;double Ǔ=60;const
float ǅ=60f;Ç ç;List<IMyShipController>Ư=new List<IMyShipController>();IMyShipController ƥ;long Ʀ;MatrixD Ŭ;MatrixD Ƨ;public
List<IMyTerminalBlock>ƨ=new List<IMyTerminalBlock>();public IMyTerminalBlock Ʃ;Vector3D ƪ=Vector3D.Zero;Vector3D?ƫ=Vector3D.
Zero;Vector3D Ƭ=Vector3D.Zero;Vector3D ƭ;Ŧ Ʈ;ō ư;ō ƻ;ō Ʊ;IDictionary<MyDetectedEntityInfo,float>Ʋ=new Dictionary<
MyDetectedEntityInfo,float>();public bool Ƴ=false;public bool ƴ=false;public bool Ƶ=false;public bool ƶ=false;Vector3D Ʒ;
MyDetectedEntityInfo Ƹ;IMyProgrammableBlock ƹ;IMyGridTerminalSystem ƺ;public ǂ(Ç Ƽ,List<IMyTerminalBlock>Ƥ,IDictionary<MyDetectedEntityInfo,
float>ŵ,IMyProgrammableBlock ļ,IMyGridTerminalSystem ķ,string ĸ,string Ĺ){ç=Ƽ;ƨ=Ƥ;Ʋ=ŵ;ƹ=ļ;ƺ=ķ;Ǆ=ĸ;ǃ=Ĺ;if(!ƴ){ń(ļ);}}public
void ĺ(string Ļ,IMyProgrammableBlock ļ){if(!ƴ){ń(ļ);}Ľ(Ļ);Ŭ=ƥ.WorldMatrix;if(ç.y(ƹ.CubeGrid.EntityId)&&Ƶ){Ƹ=(
MyDetectedEntityInfo)ç.ÿ(ƹ.EntityId,0);if(!Ƹ.IsEmpty()&&ç.f(Ʃ,Ƹ.EntityId,false,false)){Ʀ=Ƹ.EntityId;if(!ƶ){Ʈ.ů(true);ƶ=true;}Ʈ.Ţ(ƥ.
RollIndicator*30);if(ƹ.CubeGrid.GetCubeBlock(Ʃ.Position)==null||!ç.W(Ʃ,0,true,false)){Ʃ=Ł(ƹ);}try{ƫ=ç.I(Ʃ,Ʀ,0);if(ƫ!=null){ƪ=ƫ.Value;
Ƭ=ƪ-Ʃ.GetPosition();}else{ƪ=Ƹ.Position;Ƭ=ƪ-Ʃ.GetPosition();}}catch(InvalidOperationException ioe){Ƴ=false;Ƶ=false;return;
}catch(Exception e){throw e;}Ƨ=MatrixD.CreateLookAt(Vector3D.Zero,Ŭ.Forward,Ŭ.Up);ƭ=Vector3D.Normalize(Vector3D.
TransformNormal(Ƭ,Ƨ));Ĳ(ƭ);}else if(ƶ){Ʈ.ů(false);ƶ=false;}}else if(ƶ){Ʈ.ů(false);ƶ=false;}}public void Ľ(string ľ){if(ľ.ToLower().
Equals("toggle")){Ƶ=!Ƶ;}if(ľ.ToLower().Equals("fire")){Ƴ=!Ƴ;}}private bool Ŀ(IMyTerminalBlock ŀ){return ŀ.CustomName.ToLower()
.Contains("reticule");}IMyTerminalBlock Ł(IMyProgrammableBlock Ń){for(int Į=ƨ.Count-1;Į>=0;Į--){if(Ń.CubeGrid.
GetCubeBlock(ƨ[Į].Position)==null){ƨ.RemoveAt(Į);continue;}if(ç.W(ƨ[Į],0,false,false)){return ƨ[Į];}}if(ƨ.Find(Ŀ)==null){return ƨ.
Last();}else{return ƨ.Find(Ŀ);}}private void ń(IMyProgrammableBlock Ń){try{ĭ();ƨ=ĥ(ǃ,ƺ);Ŭ=ƥ.WorldMatrix;List<IMyGyro>ī=Ī(Ǆ,ƺ
);Ʈ=new Ŧ(ī,ref Ŭ);Ņ(ƹ);ƴ=true;Ʃ=Ł(Ń);}catch(Exception ex){this.ƹ.CustomData+="Aimbot failed to initialize!";this.ƹ.
CustomData+=ex.ToString();ƴ=false;return;}}void Ņ(IMyProgrammableBlock Ń){if(ǌ+Ǎ+ǎ<0.001){if(Ń.CubeGrid.ToString().Contains(
"Large")){ǌ=ǉ;Ǎ=Ǌ;ǎ=ǋ;}else{ǌ=ǒ;Ǎ=Ǉ;ǎ=ǈ;Ǐ*=2;}}ư=new ō(ǌ,Ǎ,ǎ,Ǒ,-Ǒ,ǅ);ƻ=new ō(ǌ,Ǎ,ǎ,Ǒ,-Ǒ,ǅ);Ʊ=new ō(ǌ,Ǎ,ǎ,Ǒ,-Ǒ,ǅ);}bool ņ(
Vector3D ĵ,Vector3D Ģ,double ĳ){double Ň=Math.Atan(ĳ/ĵ.Length());double ň=Math.Acos(Vector3D.Dot(ĵ,Ģ)/(ĵ.Length()*Ģ.Length()));
return Ň>ň?true:false;}bool ŉ(Vector3D ĵ,Vector3D Ģ,double ĳ){double ģ=Vector3D.Dot(ĵ,Ģ);double Ĥ=ĵ.LengthSquared();return 1<=
(ĳ*ĳ/Ĥ+1d)*ģ*ģ/Ĥ?true:false;}List<IMyTerminalBlock>ĥ(string Ħ,IMyGridTerminalSystem ħ){try{IMyBlockGroup Ĩ=ħ.
GetBlockGroupWithName(Ħ);List<IMyTerminalBlock>ĩ=new List<IMyTerminalBlock>();Ĩ.GetBlocksOfType(ĩ,m=>m.IsFunctional);return ĩ;}catch(
Exception ex){ƹ.CustomData+="Aimbot threw error in GetBlocksInGroup!";ƹ.CustomData+=ex;return null;}}List<IMyGyro>Ī(string Ħ,
IMyGridTerminalSystem ħ){try{IMyBlockGroup Ĩ=ħ.GetBlockGroupWithName(Ħ);List<IMyGyro>ī=new List<IMyGyro>();Ĩ.GetBlocksOfType(ī,Ĭ=>Ĭ.
IsFunctional);return ī;}catch(Exception ex){ƹ.CustomData+="Aimbot threw error in GetGyroscopes!";ƹ.CustomData+=(ex.ToString());
return null;}}void ĭ(){Ư=į<IMyShipController>(ƺ);ƥ=Ư[0];if(!ƥ.IsUnderControl){for(int Į=0;Į<Ư.Count;Į++){if(Ư[Į].
IsUnderControl){ƥ=Ư[Į];break;}}}}List<ĝ>į<ĝ>(IMyGridTerminalSystem ħ)where ĝ:class,IMyTerminalBlock{List<ĝ>ĩ=new List<ĝ>();ħ.
GetBlocksOfType<ĝ>(ĩ);return ĩ;}int İ(double ı){return(ı<0?-1:1);}void Ĳ(Vector3D ł){Vector3D Ĵ=new Vector3D(ł.GetDim(0),0,ł.GetDim(2))
;Vector3D Ŋ=new Vector3D(0,ł.GetDim(1),ł.GetDim(2));Ĵ.Normalize();Ŋ.Normalize();double ţ=Math.Acos(Ĵ.Dot(Vector3D.Forward
))*İ(ł.GetDim(0));double Ť=Math.Acos(Ŋ.Dot(Vector3D.Forward))*İ(ł.GetDim(1));ţ=ư.Ŝ(ţ,2);Ť=ƻ.Ŝ(Ť,2);if(Math.Abs(ţ)+Math.
Abs(Ť)>Ǔ){double ť=Ǔ/(Math.Abs(ţ)+Math.Abs(Ť));ţ*=ť;Ť*=ť;}Ʈ.ű((float)ţ);Ʈ.ų((float)Ť);}public class Ŧ{List<IMyGyro>ī;string
[]ŧ={"Yaw","Yaw","Pitch","Pitch","Roll","Roll"};private byte[]Ũ;private byte[]ũ;private byte[]Ū;public Ŧ(List<IMyGyro>ū,
ref MatrixD Ŭ){ī=ū;Ũ=new byte[ū.Count];ũ=new byte[ū.Count];Ū=new byte[ū.Count];int ŋ=0;foreach(IMyGyro Ĭ in ū){if(Ĭ!=null){
Ũ[ŋ]=ŭ(Ĭ.WorldMatrix.GetClosestDirection(Ŭ.Up));ũ[ŋ]=ŭ(Ĭ.WorldMatrix.GetClosestDirection(Ŭ.Left));Ū[ŋ]=ŭ(Ĭ.WorldMatrix.
GetClosestDirection(Ŭ.Forward));ŋ++;}}}public byte ŭ(Base6Directions.Direction Ů){switch(Ů){case Base6Directions.Direction.Forward:return 0
;case Base6Directions.Direction.Backward:return 1;case Base6Directions.Direction.Left:return 2;case Base6Directions.
Direction.Right:return 3;case Base6Directions.Direction.Up:return 4;case Base6Directions.Direction.Down:return 5;}return 0;}
public void ů(bool Ű){foreach(IMyGyro Ĭ in ī){Ĭ.GyroOverride=Ű;}}public void ű(float Ų){for(int Į=0;Į<ī.Count;Į++){byte ŋ=Ũ[Į]
;ī[Į].SetValue(ŧ[ŋ],(ŋ%2==0?Ų:-Ų)*MathHelper.RadiansPerSecondToRPM);}}public void ų(float Ŵ){for(int Į=0;Į<ī.Count;Į++){
byte ŋ=ũ[Į];if(ŋ<2){ī[Į].Roll=(ŋ%2==0?-Ŵ:Ŵ);}else if(ŋ<4){ī[Į].Pitch=(ŋ%2==0?-Ŵ:Ŵ);}else{ī[Į].Yaw=(ŋ%2==0?Ŵ:-Ŵ);}}}public
void Ţ(float Ŕ){for(int Į=0;Į<ī.Count;Į++){byte ŋ=Ū[Į];if(ŋ<2){ī[Į].Roll=(ŋ%2==0?Ŕ:-Ŕ);}else if(ŋ<4){ī[Į].Pitch=(ŋ%2==0?Ŕ:-Ŕ
);}else{ī[Į].Yaw=(ŋ%2==0?-Ŕ:Ŕ);}}}public void Ō(){foreach(IMyGyro Ĭ in ī){Ĭ.Yaw=0;Ĭ.Pitch=0;Ĭ.Roll=0;}}}public class ō{
double Ŏ;double ŏ;double Ő;double ő;double Œ;double œ;double ŕ;double Š;public ō(double Ŗ,double ŗ,double Ř,double ř=0,double
Ś=0,float ś=60f){Ő=Ŗ;ő=ŗ;Œ=Ř;œ=ř;ŕ=Ś;Š=ś;}public double Ŝ(double ŝ,int Ş){double ş=Math.Round(ŝ,Ş);Ŏ=Ŏ+(ŝ/Š);Ŏ=(œ>0&&Ŏ>œ?
œ:Ŏ);Ŏ=(ŕ<0&&Ŏ<ŕ?ŕ:Ŏ);double ʫ=(ş-ŏ)*Š;ŏ=ş;return(Ő*ŝ)+(ő*Ŏ)+(Œ*ʫ);}public void š(){Ŏ=ŏ=0;}}