using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using AvalonStudio.Commands.Settings;
using AvalonStudio.Shell.Extensibility.Platforms;

namespace AvalonStudio.Commands
{
	[Export]
	[Shared]
	public class CommandService
	{
		private readonly CommandSettingsService _commandSettingsService;
		private readonly IEnumerable<Lazy<CommandDefinition, CommandDefinitionMetadata>> _commands;

		private readonly Lazy<IImmutableDictionary<string, Lazy<CommandDefinition>>> _resolvedCommands;

		private IImmutableDictionary<CommandDefinition, string> _keyGestures;

		[ImportingConstructor]
		public CommandService(
			CommandSettingsService commandSettingsService,
			[ImportMany] IEnumerable<Lazy<CommandDefinition, CommandDefinitionMetadata>> commands)
		{
			_commandSettingsService = commandSettingsService;
			_commands = commands;

			_resolvedCommands = new Lazy<IImmutableDictionary<string, Lazy<CommandDefinition>>>(ResolveCommands);
		}

		public Lazy<CommandDefinition> GetCommand(string commandName)
		{
			var resolvedCommands = _resolvedCommands.Value;

			if (!resolvedCommands.TryGetValue(commandName, out var command))
			{
				// todo: log warning
			}

			return command;
		}

        public string GetGesture (CommandDefinition definition)
        {
            if(_keyGestures.ContainsKey(definition))
            {
                return _keyGestures[definition];
            }

            return null;
        }

		public IImmutableDictionary<CommandDefinition, string> GetKeyGesture()
		{
			if (_keyGestures != null)
			{
				return _keyGestures;
			}

			var commandSettings = _commandSettingsService.GetCommandSettings();
			var builder = ImmutableDictionary.CreateBuilder<CommandDefinition, string>();

			foreach (var command in _commands)
			{
				if (command.Value != null)
				{
					if (!commandSettings.Commands.TryGetValue(command.Metadata.Name, out var settings))
					{
						settings = new Command();

                        var gesture = command.Metadata.DefaultKeyGesture;

                        switch(Platform.PlatformIdentifier)
                        {
                            case PlatformID.MacOSX:
                                if(command.Metadata.OSXKeyGesture != null)
                                {
                                    gesture = command.Metadata.OSXKeyGesture;
                                }
                                break;

                            case PlatformID.Unix:
                                if (command.Metadata.LinuxKeyGesture != null)
                                {
                                    gesture = command.Metadata.LinuxKeyGesture;
                                }
                                break;

                            case PlatformID.Win32NT:
                                if (command.Metadata.WindowsKeyGesture != null)
                                {
                                    gesture = command.Metadata.WindowsKeyGesture;
                                }
                                break;
                        }

                        settings.Gesture = gesture;

						commandSettings.Commands.Add(command.Metadata.Name, settings);
					}

					builder.Add(command.Value, settings.Gesture);
				}
			}

			return _keyGestures = builder.ToImmutable();
		}

		private IImmutableDictionary<string, Lazy<CommandDefinition>> ResolveCommands()
		{
			var builder = ImmutableDictionary.CreateBuilder<string, Lazy<CommandDefinition>>();

			foreach (var command in _commands)
			{
				builder.Add(command.Metadata.Name, command);
			}

			return builder.ToImmutable();
		}
	}
}
