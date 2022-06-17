using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEngine;

namespace TP.VisualNovel {
    public class TPCommand {
        private const string CMD_RE = @"(?<=\s*)(?<name>\w+)\s*\((?<factor>\s*(?:(?:""""[\s\S]*?"""")|[\w.\d]*?)\s*(?:,\s*(?:(?:""""[\s\S]*?"""")|[\w.\d]+?)\s*)*)\);";
        private const string CMD_SPLIT_RE = ",(?=(?:[^\"\"]*\"\"[^\"\"]*\"\")*[^\"\"]*$)";
        private const string CMD_REPLACE_RE = @"^\s*""""|""""\s*$";

        private string name;
        private object[] factors;

        public bool IsEnd { get; private set; }
        public static TPCommand runningCmd;
        public TPCommand(string _name, string _factor) {
            name = _name;
            if (string.IsNullOrEmpty(_factor)) {
                factors = null;
            }
            else {
                string[] _factors = Regex.Split(_factor, CMD_SPLIT_RE);
                factors = new object[_factors.Length];
                for (int i = 0; i < factors.Length; ++i) {
                    string _stringValue = Regex.Replace(_factors[i], CMD_REPLACE_RE, "");
                    bool isFloat = float.TryParse(_stringValue, out float _floatValue);
                    bool isBool = bool.TryParse(_stringValue, out bool _boolValue);
                    bool isString = isFloat == false && isBool == false;
                    if (isString) {
                        factors[i] = _stringValue;
                    }
                    else if (isBool) {
                        factors[i] = _boolValue;
                    }
                    else if (isFloat) {
                        factors[i] = _floatValue;
                    }
                }
            }
            IsEnd = false;
        }
        public override string ToString() {
            string str = $"Name : {name} ";
            if (factors == null) str += "No Factors";
            else {
                str += "Factors { ";
                for (int i = 0; i < factors.Length; ++i) {
                    str += factors[i] + (i < factors.Length - 1 ? ": " : "");
                }
                str += " }";
            }
            return str;
        }

        public void Execute(object target) {
            Type type = typeof(Master_VisualNovel);
            MethodInfo method = type.GetMethod(name);
            if (method == null) {
                Debug.Log($"해당하는 Method가 존재하지 않습니다 : {name}");
                return;
            }
            runningCmd = this;
            Event.Global_EventSystem.VisualNovel.onCommandEnd += OnCommandEnd;
            try {
                method.Invoke(target, factors);
            }
            catch (Exception e) {
                Debug.Log($"인자값 문제 발생 : {e}\n 대체 실행 : {name}");
                ParameterInfo[] infos = method.GetParameters();
                for (int i = 0; i < infos.Length; ++i) {
                    if (infos[i].ParameterType == typeof(float[])) {
                        float[] newFactors = new float[factors.Length];
                        for (int j = 0; j < newFactors.Length; ++j) {
                            newFactors[j] = (float)factors[j];
                        }
                        method.Invoke(target, new object[] { newFactors });
                        break;
                    }
                    else if (infos[i].ParameterType == typeof(string)) {
                        object[] newFactors = new object[factors.Length];
                        for (int j = 0; j < newFactors.Length; ++j) {
                            newFactors[j] = $"{factors[j]}";
                        }
                        method.Invoke(target, newFactors);
                        break;
                    }
                }
            }
        }

        public static List<TPCommand> Build(string _data) {
            Regex cmdRegex = new Regex(CMD_RE);
            List<TPCommand> myCmds = new List<TPCommand>();
            foreach (Match match in cmdRegex.Matches(_data)) {
                TPCommand cmd = new TPCommand(match.Groups["name"].Value, match.Groups["factor"].Value);
                myCmds.Add(cmd);
            }
            return myCmds;
        }

        private void OnCommandEnd() {
            IsEnd = true;
            Event.Global_EventSystem.VisualNovel.onCommandEnd -= OnCommandEnd;
        }
    }




}