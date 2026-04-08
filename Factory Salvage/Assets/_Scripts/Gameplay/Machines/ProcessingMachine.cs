using System.Collections.Generic;
using UnityEngine;
using FactorySalvage.Data;

namespace FactorySalvage.Gameplay
{
    /// <summary>
    /// A machine that processes recipes: consumes inputs, produces outputs.
    /// </summary>
    public class ProcessingMachine : MachineBase
    {
        #region Fields

        [Header("Processing")]
        [SerializeField] private RecipeDefinition _recipe;

        [Header("Energy")]
        [SerializeField] private EnergyConsumer _energyConsumer;

        private readonly Dictionary<ResourceDefinition, int> _inputBuffer = new();
        private readonly Dictionary<ResourceDefinition, int> _outputBuffer = new();
        private float _processTimer;
        private bool _isProcessing;

        #endregion

        #region Properties

        public RecipeDefinition Recipe => _recipe;
        public bool IsProcessing => _isProcessing;
        public IReadOnlyDictionary<ResourceDefinition, int> InputBuffer => _inputBuffer;
        public IReadOnlyDictionary<ResourceDefinition, int> OutputBuffer => _outputBuffer;

        #endregion

        #region Unity Callbacks

        private void Update()
        {
            if (!IsPlaced) return;

            // No power = no processing
            if (_energyConsumer != null && !_energyConsumer.HasPower) return;

            if (_isProcessing)
            {
                _processTimer -= Time.deltaTime;
                if (_processTimer <= 0f)
                {
                    CompleteProcessing();
                }
            }
            else
            {
                TryStartProcessing();
            }
        }

        #endregion

        #region Public Methods

        public void SetRecipe(RecipeDefinition recipe)
        {
            _recipe = recipe;
        }

        public bool AcceptInput(ResourceDefinition resource, int amount)
        {
            if (resource == null || amount <= 0) return false;

            if (_inputBuffer.ContainsKey(resource))
            {
                _inputBuffer[resource] += amount;
            }
            else
            {
                _inputBuffer[resource] = amount;
            }
            return true;
        }

        public bool TryTakeOutput(out ResourceDefinition resource, out int amount)
        {
            foreach (var kvp in _outputBuffer)
            {
                if (kvp.Value > 0)
                {
                    resource = kvp.Key;
                    amount = kvp.Value;
                    _outputBuffer.Remove(kvp.Key);
                    return true;
                }
            }

            resource = null;
            amount = 0;
            return false;
        }

        public bool HasOutput()
        {
            foreach (var kvp in _outputBuffer)
            {
                if (kvp.Value > 0) return true;
            }
            return false;
        }

        public override void Interact(PlayerController player)
        {
            // Manual feed: player gives resources from inventory
            if (_recipe == null) return;

            var inventory = player.GetComponent<Inventory>();
            if (inventory == null) return;

            foreach (var input in _recipe.Inputs)
            {
                int currentInBuffer = GetBufferAmount(_inputBuffer, input.Resource);
                int needed = input.Amount - currentInBuffer;

                if (needed > 0 && inventory.HasEnoughResource(input.Resource, needed))
                {
                    inventory.RemoveResource(input.Resource, needed);
                    AcceptInput(input.Resource, needed);
                }
            }
        }

        public override string GetInteractionPrompt()
        {
            if (_recipe == null) return base.GetInteractionPrompt();
            return _isProcessing
                ? $"{Definition?.MachineName}: Processing..."
                : $"{Definition?.MachineName}: Feed resources";
        }

        #endregion

        #region Private Methods

        private void TryStartProcessing()
        {
            if (_recipe == null) return;

            // Check all inputs available
            foreach (var input in _recipe.Inputs)
            {
                int available = GetBufferAmount(_inputBuffer, input.Resource);
                if (available < input.Amount) return;
            }

            // Consume inputs
            foreach (var input in _recipe.Inputs)
            {
                ConsumeFromBuffer(_inputBuffer, input.Resource, input.Amount);
            }

            _processTimer = _recipe.ProcessTime;
            _isProcessing = true;
        }

        private void CompleteProcessing()
        {
            if (_recipe == null) return;

            // Add outputs
            foreach (var output in _recipe.Outputs)
            {
                if (_outputBuffer.ContainsKey(output.Resource))
                {
                    _outputBuffer[output.Resource] += output.Amount;
                }
                else
                {
                    _outputBuffer[output.Resource] = output.Amount;
                }
            }

            _isProcessing = false;
        }

        private int GetBufferAmount(Dictionary<ResourceDefinition, int> buffer, ResourceDefinition resource)
        {
            return buffer.TryGetValue(resource, out int amount) ? amount : 0;
        }

        private void ConsumeFromBuffer(Dictionary<ResourceDefinition, int> buffer, ResourceDefinition resource, int amount)
        {
            if (!buffer.ContainsKey(resource)) return;
            buffer[resource] -= amount;
            if (buffer[resource] <= 0)
            {
                buffer.Remove(resource);
            }
        }

        #endregion
    }
}
