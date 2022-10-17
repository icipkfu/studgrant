namespace Grant.Core.Install
{
    using System.Collections.Generic;

    /// <summary>
    /// Интерфейс установщика
    /// </summary>
    public interface IInstaller
    {
        string Name { get; }

        IEnumerable<string> Dependencies { get; }

        /// <summary>
        /// Запустить установщик
        /// </summary>
        void Install();
    }
}