using Avalonia.Media;
using AvalonStudio.Commands;
using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace AvalonStudio.Menus
{
    internal class CommandMenuItem : IMenuItem
    {
        private Lazy<IEnumerable<string>> _gestures;

        public string Label => _commandDefinition.Value?.Label;
        public DrawingGroup Icon => _commandDefinition.Value?.Icon;

        public ICommand Command => _commandDefinition.Value?.Command;

        public IEnumerable<string> Gestures => _gestures?.Value;

        private readonly CommandService _commandsService;
        private readonly string _commandName; 

        private readonly Lazy<CommandDefinition> _commandDefinition;

        public CommandMenuItem(CommandService commandsService, string commandName)
        {
            _commandsService = commandsService;
            _commandName = commandName;

            _commandDefinition = new Lazy<CommandDefinition>(ResolveCommandDefinition);

            _gestures = new Lazy<IEnumerable<string>>(() =>
            {
                return _commandsService.GetGestures(_commandDefinition.Value);
            });
        }

        private CommandDefinition ResolveCommandDefinition() => _commandsService.GetCommand(_commandName)?.Value;
    }
}
