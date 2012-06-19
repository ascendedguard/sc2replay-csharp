One huge little-endian bitstream.  Each event has a header as follows:

* 2 bits - Time interval length
* 6, 14, 22, or 30 bits - Time interval amount in ticks (After 2 days of idling, 30 bits is reached--silly)
* 5 bits - Player id -  0x10 means global.
* 7 bits - Event type


## Event types

This should be generally sparse because the same codes are used for networking.

* 0x05 - Game start
* 0x0c - Join game
* 0x19 - Leave game
* 0x1b - Ability
* 0x1c - Selection
* 0x1d - Control Groups
* 0x1f - Resource trading
* 0x23 - ???
* 0x26 - ???
* 0x31 - Camera
* 0x46 - Request resources (Computer)


## Serialized Contents:

* All CAbil types and all possible fields
* Camera Movement
* Unit Selection
* Control Groups
* Resource Trading


### CAbil Types:

    0x0     CAbil                   (prototype)
    0x1     CAbilEffect             (prototype)
    0x2     CAbilQueueable          (dummy)
    0x3     CAbilProgress           (dummy)
    0x4     CAbilRedirect           (internal -- used by behaviors only)
    0x5     CAbilArmMagazine        ArmSiloWithNuke, ArmInterceptor (InfoArray, just 1) (others are passive)
    0x6     CAbilAttack             Attack, simple button with unit/location target
    0x7     CAbilAugment            Charge (passive, could be alt-ed I guess?)
    0x8     CAbilBattery            (unused)
    0x9     CAbilBeacon             (not seen on replays)
    0xA     CAbilBehavior           Banshee Cloak.  Likely just default target (0), on/off flag.
    0xB     CAbilBuild              Any building creation.  Target is location or default
    0xC     CAbilBuildable          Build in progress.  Has cancel button.
    0xD     CAbilEffectInstant      Stimpack.  Target can be default, location, or unit.
    0xE     CAbilEffectTarget       E.g. Blink.  Target can be location or unit, default is generally invalid.
    0xF     CAbilHarvest            Gathering.  Target is unit.  Also has "Return Cargo", always default target
    0x10    CAbilInteract           (Not used in multiplayer, target is unit)
    0x11    CAbilInventory          (...)
    0x12    CAbilLearn              (internal? or unused)
    0x13    CAbilMerge              Archons.  Target generally default.
    0x14    CAbilMergeable          High templar?
    0x15    CAbilMorph              Terran lifts, siege mode, command center => orbital/pf, zerg units,
                                    burrow.  Causes selection update
    0x16    CAbilMorphPlacement     Terran lands, spine/spore placement.  Has location target as opposed
                                    to default.
    0x17    CAbilMove               Move, Patrol, Hold Position, (acquire move?, turn?)
    0x18    CAbilPawn               (unused)
    0x19    CAbilQueue              Production queues.  Handles canceling from CAbilTrain/Research
    0x1A    CAbilRally              Rally
    0x1B    CAbilResearch           Research (Generally CUpgrades)
    0x1C    CAbilRevive             (unused)
    0x1D    CAbilSpecialize         (unused)
    0x1E    CAbilStop               Stop, (Hold fire internal?), /cheer, /dance
    0x1F    CAbilTrain              Unit training -- 0~0x1d = train, 0x1e = cancel, 0x1f = ?
    0x20    CAbilTransport          Unitception.  Kinda weird cause default ability for moving
                                    to certain unit targets (e.g bunker) is Transport, but that's
                                    a backwards reference.
    0x21    CAbilWarpable           CAbilBuildable for protoss, handles canceling
    0x22    CAbilWarpTrain          Warp in unit using warp gates (not! production queues)
                                    Handles canceling.

### Camera Movement

Camera x, y, distance, pitch, yaw, and height(?)

### Unit Selection

Possible active transitions in the wireframe:

* Player selects additional units (adds to current selection)
* Player selects new units (replacing current selection)
* Player deselects units (shift, ctrl-shift click in the wireframe)

Also, some passive transitions are saved to replay, specifically those created by CAbilMorphs.

### Control Groups

* Add current selection to control group (shift-#)
* Replace control group with current selection (ctrl-#)
* Select control group (#)

### Mineral Trading

* Sending minerals, gas, terrazine, custom resource

## Specific bitfields

After every packet, the bitstream cursor is aligned to the next byte.

### Basic types

- fixed32: A 32 bit long signed fixed-float point field using a sign bit.  The integral portion is 19 bits and the fractional portion is 12.
- CFixed<size>: A size bits long unsigned fixed-float field.  Technically, the actual CFixed is only 32 bits and is signed, but I'm using it here to differentiate between the fixed32 type.
    - CFixed<20>: 8.12 bits
    - CFixed<16>: 8.8 bits
- CRotationAmount: A 16-bit long integer representation of rotation.  To convert the 16-bit amount to degrees, use the following formula:
    - `degrees = 45 * (((((amount * 0x10 - 0x2000) << 17) - 1) >> 17) + 1) / 4096.0`

### 0x05 - Game start

Nothing here.  Player is global.

### 0x0c - Join game

- ` 1 bit `: (`0`)
- ` 1 bit `: (`1 in Editor Test, 0 in Battle.net`)
- ` 1 bit `: (`0`)
- ` 1 bit `: (`0`)

### 0x19 - Leave game

Nothing here.

### 0x1b - Ability

- `18 bits`: Flags:
    - `001`: 
    - `002`: Queued ability (holding shift)
    - `004`: 
    - `008`: Right click
    - `010`: 
    - `020`: Wireframe button
    - `040`: Toggle auto
    - `080`: Turning on auto
    - `100`: Always `1`
    - `200`: Wireframe unit unload
    - `400`: Wireframe unit cancel (as opposed to Esc)
    - `10000`: Minimap target
    - `20000`: Failed -- This only really shows up when several actions are attempted in a single tick through hotkeys
- ` 1 bit `: Default ability? (i.e. right click)
    - `16 bits`: Ability Id
    - ` 5 bits`: Button index (InfoArray, ButtonArray in Editor)
    - ` 1 bit `: (`0`)
- ` 2 bits`: Target type:
    - `0`: Default target
    - `1`: Location target
        - `20 bits`: Target X
        - `20 bits`: Target Y
        - `32 bits`: Target Z (fixed32)
    - `2`: Unit + Location target
        - ` 8 bits`: (`0x37`)
        - ` 8 bits`: (`0`)
        - `32 bits`: Unit Id
        - `16 bits`: Unit Type
        - ` 1 bit `: Target has player?
            - ` 4 bits`: Target player
        - ` 1 bit `: Target has team?
            - ` 4 bits`: Target team
        - `20 bits`: Target X (CFixed)
        - `20 bits`: Target Y (CFixed)
        - `32 bits`: Target Z (fixed32)
    - `3`: Unit target
        - `32 bits`: Unit Id -- For queues, id>>18 == 0 and low 18 bits is a globally incrementing id.  This means to figure out which research or unit was canceled requires keeping track of everything that's been queued.  Completion status has no effect on the id.
- ` 1 bit `: (`0`)

### 0x1c - Selection

- ` 4 bits`: flags?
    - `0xA`: Selection change
    - `0x5`: Unit change
- ` 8 bits`: Always `0`
- ` 2 bits`: Update flags:
    - `0`: Add to selection (ctrl) - No wireframe data
    - `1`: Remove from selection (shift-click or click wireframe or unit dies) - Wireframe is removal
    - `2`: ? (Use same as 3 without wireframe index -- this is most likely, but unconfirmed)
    - `3`: Replace selection (everything else) - Wireframe is addition
- Flags equal 0:
    - ` 8 bits`: Unit type length
        - `16 bits`: Unit type id
        - ` 8 bits`: (`1`)
        - ` 8 bits`: Number of units of that type
    - ` 8 bits`: Unit id length
        - `32 bits`: Unit id
- Flags equal 1:
    - ` 8 bits`: Number of units affected (size of next field in bits)
    - ` n bits`: Removed unit flags; 1 => unit removed.  The msb is the first unit in the wireframe.
    - ` 8 bits`: Types? (If `1`, it's adding to the selection)
        - `16 bits`: Unit type id
        - ` 8 bits`: (`1`)
        - ` 8 bits`: Number of units of that type
    - ` 8 bits`: Ids? (If `1`, it's adding to the selection)
        - `32 bits`: Unit id
- Flags equal 3:
    - ` 8 bits`: Wireframe index length (i.e. we have replaced our selection with a unit in the wireframe)
        - ` 8 bits`: Wireframe index of unit
    - ` 8 bits`: Unit type length
        - `16 bits`: Unit type id
        - ` 8 bits`: (`1`)
        - ` 8 bits`: Number of units of that type
    - ` 8 bits`: Unit id length
        - `32 bits`: Unit id

### 0x1d - Control Groups

- ` 4 bits`: Control group
- ` 2 bits`: Action type:
    - `0`: Set current selection as control group (ctrl-#)
    - `1`: Add current selection to control group (shift-#)
    - `2`: Selection control group (#)
    - `3`: (Would be) Add control group to current selection
- ` 2 bits`: Unknown (`0`)

### 0x1f - Resource trading

- ` 4 bits`: Player id of sendee
- ` 3 bits`: (Only seen `4`)
- `32 bits`: `fixed32` Mineral amount
- `32 bits`: `fixed32` Vespene amount
- `32 bits`: `fixed32` Terrazine amount
- `32 bits`: `fixed32` Custom amount

### 0x23 - ???

No idea.  Happened after resuming process in debugger, so perhaps a sync event.

- ` 8 bits`: `0x81`

### 0x26 - ???

Happened in a 2v2 with computers.

- `32 bits`: (`4`)
- `32 bits`: (`0`)

### 0x31 - Camera


- `16 bits`: CFixed<16> X
- `16 bits`: CFixed<16> Y
- ` 1 bit `: bool Distance?
    - `16 bits`: CFixed<16> Distance
- ` 1 bit `: bool Pitch?
    - `16 bits`: CRotationAmount<16> Pitch
        - Note:  this angle is relative to the horizontal plane, but the editor shows the angle relative to the vertical plane.  Subtract from 90 degrees to convert.
- ` 1 bit `: bool Yaw?
    - `16 bits`: CRotationAmount<16> Yaw
        - Note:  this angle is the vector from the camera head to the camera target projected on to the x-y plane in positive coordinates.  So, default is 90 degrees, while insert and delete produce 45 and 135 degrees by default.
- ` 1 bit `: bool Unknown? (Maybe height offset?)
    - `?? bits`: ? ?

### 0x46 - Request resources (Computer)

Nothing important about this, but...

- ` 3 bits`: (Only seen `4`)
- `32 bits`: `fixed32` Mineral amount
- `32 bits`: `fixed32` Vespene amount
- `32 bits`: `fixed32` Terrazine amount
- `32 bits`: `fixed32` Custom amount

