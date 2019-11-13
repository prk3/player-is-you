using System;
using System.Collections.Generic;
using System.Linq;
using Entities;
using UnityEngine;
using Traits;

/**
 * Loads map from level_.bytes file, renders tiles, parses and applies rules.
 * IMPORTANT: tiles exist in a plane where Y axis grows downwards (like in images).
 */
public class Map : MonoBehaviour
{
    public int levelId;
    public int width;
    public int height;
    public List<Entity>[][] stacks;
    private List<(EntityType, EntityType)> _prevRules;

    private ObjectFitContain _contain;

    void Start()
    {
        // clear mod cache so that we don't use too much memory
        TileMod.ClearModCache();

        string levelFile = $"Levels/{levelId}";
        byte[] levelArray = Resources.Load<TextAsset>(levelFile).bytes;

        Debug.Assert(levelArray.Length >= 2);

        width = levelArray[0];
        height = levelArray[1];

        Debug.Assert(levelArray.Length == width * height + 2);

        var mapBackground = GameObject.CreatePrimitive(PrimitiveType.Plane);
        mapBackground.name = "background";
        mapBackground.transform.parent = gameObject.transform;
        mapBackground.transform.localPosition = new Vector3(width / 2.0f, -height / 2.0f, 1);

        mapBackground.transform.localScale = new Vector3(width / 10.0f, 1, height / 10.0f);

        var rotation = mapBackground.transform.localRotation.eulerAngles;
        rotation.x = -90;
        mapBackground.transform.localRotation = Quaternion.Euler(rotation);

        mapBackground.GetComponent<MeshRenderer>().material.color = new Color(0.08f, 0.08f, 0.08f, 1f);
        mapBackground.GetComponent<MeshRenderer>().material.shader = Shader.Find("UI/Default");

        stacks = new List<Entity>[height][];

        for (int y = 0; y < height; y++)
        {
            stacks[y] = new List<Entity>[width];
            for (int x = 0; x < width; x++)
            {
                stacks[y][x] = new List<Entity>(2); // entities rarely stack in more than 2
                var id = levelArray[(y * width) + x + 2];

                // we skip id 0, which encodes an empty tile
                if (id == 0) continue;

                EntityType type;

                try
                {
                    type = (EntityType) id;
                }
                catch (InvalidCastException)
                {
                    Debug.LogWarning($"Invalid entity type {id} in map file {levelFile}");
                    continue;
                }

                var tile = MakeEntityObject(x, y, type);
                stacks[y][x].Add(tile.GetComponent<Entity>());
            }
        }

        _prevRules = new List<(EntityType, EntityType)>();
        UpdateRules();

        _contain = gameObject.AddComponent<ObjectFitContain>();
    }

    void Update()
    {
        if (_contain.Contain(width, height))
        {
            // invert Y transform
            var localPos = gameObject.transform.localPosition;
            gameObject.transform.localPosition = new Vector3(localPos.x, -localPos.y, localPos.z);
        }
    }

    /**
     * Creates an entity with position x, y and type "type".
     */
    private GameObject MakeEntityObject(int x, int y, EntityType type)
    {
        var obj = new GameObject("entity");
        obj.transform.parent = gameObject.transform;

        var entity = obj.AddComponent<Entity>();
        entity.type = type;
        entity.x = x;
        entity.y = y;
        entity.z = 0;

        if (Entity.IsSubject(type) || Entity.IsTrait(type) || type == EntityType.ConnectorIs)
        {
            obj.AddComponent<Push>();
        }

        var ren = obj.AddComponent<SpriteRenderer>();
        ren.sprite = Sprite.Create(
            Entity.GetTexture(),
            new Rect(32 * (int) (type-1), 0, 32, 32),
            new Vector2(0, 0),
            32);

        obj.AddComponent<AnimatedSprite>();

        var mod = Entity.GetModTilemap(type);

        if (mod != null)
        {
            var modComp = obj.AddComponent<TileMod>();
            modComp.modTilemap = mod;
        }

        return obj;
    }

    /**
     * Whether a position is withing map's bounding box.
     */
    public bool IsValidSpot(Vector2Int pos)
    {
        return pos.x >= 0 && pos.x < width && pos.y >= 0 && pos.y < height;
    }

    /**
     * Given an entity, collect neighbours around that entity.
     * If neighbour position is out of bounds, use bitFroMissingNeighbours instead.
     * See TileMod for neighbour-bit relation info.
     */
    public byte CollectNeighborsByte(Entity e, bool bitForMissingNeighbors = false)
    {
        bool CollectStack(int x, int y)
        {
            if (!IsValidSpot(new Vector2Int(x, y))) return bitForMissingNeighbors;
            return stacks[y][x].Find(entity => entity.type == e.type);
        }

        byte output = 0;

        output |= (byte)(CollectStack(e.x - 1, e.y - 1) ?   1 : 0);
        output |= (byte)(CollectStack(e.x + 0, e.y - 1) ?   2 : 0);
        output |= (byte)(CollectStack(e.x + 1, e.y - 1) ?   4 : 0);

        output |= (byte)(CollectStack(e.x - 1, e.y + 0) ?   8 : 0);

        output |= (byte)(CollectStack(e.x + 1, e.y + 0) ?  16 : 0);

        output |= (byte)(CollectStack(e.x - 1, e.y + 1) ?  32 : 0);
        output |= (byte)(CollectStack(e.x + 0, e.y + 1) ?  64 : 0);
        output |= (byte)(CollectStack(e.x + 1, e.y + 1) ? 128 : 0);

        return output;
    }

    /**
     * Extracts all rules from the map.
     */
    private List<(EntityType, EntityType)> ExtractRules()
    {
        var output = new List<(EntityType, EntityType)>();

        foreach (var row in stacks)
        {
            foreach (var stack in row)
            {
                foreach (Entity entity in stack)
                {
                    if (entity.type == EntityType.ConnectorIs)
                    {
                        ExtractRulesFromConnectorIs(entity, output);
                    }
                }
            }
        }

        return output;
    }

    /**
     * Extracts rules with a center on "is connector" entity.
     */
    private void ExtractRulesFromConnectorIs(Entity entity, List<(EntityType, EntityType)> list)
    {
        // is connector can be a part of a vertical rule
        if (entity.y != 0 && entity.y != height - 1)
        {
            ExtractRulesFromStacks(stacks[entity.y - 1][entity.x], stacks[entity.y + 1][entity.x], list);
        }

        // is connector can be a part of a horizontal rule
        if (entity.x != 0 && entity.x != width - 1)
        {
            ExtractRulesFromStacks(stacks[entity.y][entity.x - 1], stacks[entity.y][entity.x + 1], list);
        }
    }

    /**
     * Adds rules from stacks one and two into list.
     */
    private void ExtractRulesFromStacks(List<Entity> stackOne, List<Entity> stackTwo, List<(EntityType, EntityType)> list)
    {
        foreach (var primary in stackOne)
        {
            var primaryType = primary.type;
            var isPrimarySubject = Entity.IsSubject(primaryType);
            var isPrimaryTrait = Entity.IsTrait(primaryType);

            if (!isPrimarySubject && !isPrimaryTrait) continue;

            foreach (var secondary in stackTwo)
            {
                var secondaryType = secondary.type;
                var isSecondarySubject = Entity.IsSubject(secondaryType);
                var isSecondaryTrait = Entity.IsTrait(secondaryType);

                if (isPrimarySubject && isSecondaryTrait)
                {
                    list.Add((primaryType, secondaryType));
                }

                if (isSecondarySubject && isPrimaryTrait)
                {
                    list.Add((secondaryType, primaryType));
                }
            }
        }
    }

    /**
     * Scans map for changes in rules and adds/removes trait behaviours.
     */
    public void UpdateRules()
    {
        var newRules = ExtractRules();
        var addedRules = newRules.Except(_prevRules);
        var deletedRules = _prevRules.Except(newRules);

        foreach (var (subject, trait) in deletedRules)
        {
            var targetType = Entity.GetEntityTypeFromSubject(subject);
            var entities = gameObject.GetComponentsInChildren<Entity>();

            foreach (var s in entities)
            {
                if (s.type != targetType) continue;

                var component = s.gameObject.GetComponent(Entity.GetBehaviorFromTrait(trait));
                if (component) Destroy(component);
            }
        }

        foreach (var (subject, trait) in addedRules)
        {
            var targetType = Entity.GetEntityTypeFromSubject(subject);
            var entities = gameObject.GetComponentsInChildren<Entity>();

            foreach (var entity in entities)
            {
                if (entity.type != targetType) continue;

                entity.gameObject.AddComponent(Entity.GetBehaviorFromTrait(trait));
            }
        }

        _prevRules = newRules;
    }

    /**
     * Optimizes stacks at given positions and refreshes approximate mods.
     */
    public void RefreshStacks(List<Vector2Int> positions)
    {
        var withNeighbours = new HashSet<Vector2Int>();

        void TryAddPosition(HashSet<Vector2Int> set, Vector2Int pos)
        {
            if (IsValidSpot(pos)) set.Add(pos);
        }

        foreach (var position in positions)
        {
            TryAddPosition(withNeighbours, position + Vector2Int.left   + Vector2Int.down);
            TryAddPosition(withNeighbours, position                     + Vector2Int.down);
            TryAddPosition(withNeighbours, position + Vector2Int.right  + Vector2Int.down);

            TryAddPosition(withNeighbours, position + Vector2Int.left);
            TryAddPosition(withNeighbours, position);
            TryAddPosition(withNeighbours, position + Vector2Int.right);

            TryAddPosition(withNeighbours, position + Vector2Int.left   + Vector2Int.up);
            TryAddPosition(withNeighbours, position                     + Vector2Int.up);
            TryAddPosition(withNeighbours, position + Vector2Int.right  + Vector2Int.up);
        }

        foreach (var pos in positions)
        {
            OptimizeStack(pos);
        }

        foreach (var pos in withNeighbours)
        {
            foreach (var entity in stacks[pos.y][pos.x])
            {
                var mod = entity.gameObject.GetComponent<TileMod>();
                if (mod != null) {
                    mod.ApplyMod(CollectNeighborsByte(entity, true));
                }
            }
        }
    }

    /**
     * Reapplies z value on entities based on their position in a stack.
     */
    private void OptimizeStack(Vector2Int pos)
    {
        Debug.Assert(IsValidSpot(pos));
        var stack = stacks[pos.y][pos.x];
        for (int i = 0; i < stack.Count; i++)
        {
            if (stack[i].z != i)
            {
                stack[i].z = i;
            }
        }
    }

    /**
     * Executes trait logic on an entire map.
     */
    public void ApplyRules() {
        for (int y = 0; y < height; y++)
        {
            var row = stacks[y];
            for (int x = 0; x < width; x++)
            {
                var stack = row[x];

                for (int t = 0; t < 20; t++)
                {
                    if (ApplyRulesOnStack(stack) != RuleApplicationOutcome.Refresh) goto triesEnd;
                }

                Debug.LogError($"The limit of trait updates has been reached for stack x={x}, y={y}");

                triesEnd: ;
            }
        }
    }

    /**
     * Applies trait login on one stack.
     */
    private RuleApplicationOutcome ApplyRulesOnStack(List<Entity> stack)
    {
        var traitOrderPairs = new List<(int, Trait)>();

        for (int i = 0; i < stack.Count; i++)
        {
            foreach (var trait in stack[i].GetComponents<Trait>())
            {
                traitOrderPairs.Add((trait.GetInteractionOrder() - i, trait));
            }
        }

        traitOrderPairs.Sort((a, b) => b.Item1 - a.Item1);

        foreach (var (_, trait) in traitOrderPairs)
        {
            switch (trait.ApplyRuleOnStack(stack))
            {
                case RuleApplicationOutcome.Refresh:
                    return RuleApplicationOutcome.Refresh;
                case RuleApplicationOutcome.Break:
                    return RuleApplicationOutcome.Break;
                case RuleApplicationOutcome.Continue:
                    break;
            }
        }
        return RuleApplicationOutcome.Break;
    }
}
