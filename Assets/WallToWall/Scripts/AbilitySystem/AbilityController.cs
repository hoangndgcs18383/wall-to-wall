using System.Collections.Generic;
using Utilities;

namespace FreakyBall.Abilities
{
    public class AbilityController
    {
        readonly AbilityModel _model;
        readonly AbilityView _view;
        readonly Queue<AbilityCommand> abilityCommands = new();
        readonly CountdownTimer _abilityCooldown = new CountdownTimer(0);

        public AbilityController(AbilityModel abilityModel)
        {
            _model = abilityModel;

            ConnectModel();
        }

        public AbilityController(AbilityModel abilityModel, AbilityView abilityView)
        {
            _model = abilityModel;
            _view = abilityView;

            ConnectModel();
            ConnectView();
        }
        
        public void Update(float deltaTime)
        {
            _abilityCooldown.Tick(deltaTime);
            if(_view) _view.UpdateProgress(_abilityCooldown.Progress);

            if (!_abilityCooldown.IsRunning && abilityCommands.TryDequeue(out AbilityCommand cmd))
            {
                cmd.Execute();
                _abilityCooldown.Reset(cmd.countdown);
                _abilityCooldown.Start();
            }
        }

        void ConnectModel()
        {
            _model.abilities.AnyValueChanged += OnAbilitiesChanged;
        }
        
        void ConnectView()
        {
            for (int i = 0; i < _view.abilityButtons.Length; i++)
            {
                _view.abilityButtons[i].RegisterListener(OnButtonAbilityPressed);
            }
            
            _view.UpdateSprites(_model.abilities);
        }

        private void OnButtonAbilityPressed(int index)
        {
            if (_abilityCooldown.Progress < 0.25 || !_abilityCooldown.IsRunning)
            {
                if (_model.abilities[index] != null)
                {
                    abilityCommands.Enqueue(_model.abilities[index].CreateCommand());
                }
            }
        }

        private void OnAbilitiesChanged(IList<Ability> abilities)
        {
            if (_view != null)
            {
                _view.UpdateSprites(abilities);
            }
        }

        public class Builder
        {
            readonly AbilityModel _model = new AbilityModel();

            public Builder WithAbilities(AbilityData[] abilities)
            {
                foreach (var ability in abilities)
                {
                    _model.AddAbility(new Ability(ability));
                }

                return this;
            }

            public AbilityController Build()
            {
                return new AbilityController(_model);
            }

            public AbilityController Build(AbilityView abilityView)
            {
                return new AbilityController(_model, abilityView);
            }
        }
    }
}