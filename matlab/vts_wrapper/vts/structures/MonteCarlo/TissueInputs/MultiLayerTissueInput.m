% MULTILAYERTISSUEINPUT Defines the input data for multi-layer tissue
classdef MultiLayerTissueInput < handle % deriving from handle allows us to keep a singleton around (reference based) - see Doug's post here: http://www.mathworks.com/matlabcentral/newsreader/view_thread/171344
    properties
        LayerRegions = struct(...
            'ZRange',{...
                [-Inf, 0], ...
                [0, 100], ...
                [100, +Inf]}, ...
            'RegionOP',{...
                [0.0, 1e-10, 1.0, 1.0], ...
                [0.0, 1.0, 0.8, 1.4], ...
                [0.0, 1e-10, 1.0, 1.0]  }, ...
            'PhaseFunctionKey', {...
                'HenyeyGreensteinKey1',... 
                'HenyeyGreensteinKey2',...
                'HenyeyGreensteinKey3'}...
            );
         RegionPhaseFunctionInputs = containers.Map({'HenyeyGreensteinKey1','HenyeyGreensteinKey2','HenyeyGreensteinKey3'},...
            {Vts.MonteCarlo.HenyeyGreensteinPhaseFunctionInput(),...
            Vts.MonteCarlo.HenyeyGreensteinPhaseFunctionInput(),...
            Vts.MonteCarlo.HenyeyGreensteinPhaseFunctionInput()});
    end
    
    methods (Static)
        function input = FromInputNET(inputNET)
            input = MultiLayerTissueInput;
            input.RegionPhaseFunctionInputs = inputNET.RegionPhaseFunctionInputs;
            regionNET = inputNET.Regions;
            for i=1:regionNET.Length
                regions(i).ZRange = [...
                    regionNET(i).ZRange.Start, ...
                    regionNET(i).ZRange.Stop ...
                    ];
                regions(i).RegionOP = [ ...
                    regionNET(i).RegionOP.Mua, ...
                    regionNET(i).RegionOP.Musp, ...
                    regionNET(i).RegionOP.G, ...
                    regionNET(i).RegionOP.N ...
                    ];
                regions(i).PhaseFunctionKey = ...
                    regionNET(i).PhaseFunctionKey; ... 
            end
            input.LayerRegions = regions;
        end
        
        function inputNET = ToInputNET(input)                
            regionsNET = NET.createArray('Vts.MonteCarlo.ITissueRegion', length(input.LayerRegions));
            phaseFunctionInputs = input.RegionPhaseFunctionInputs;
            phaseFunctionKeys = keys(phaseFunctionInputs);
            phaseFunctionValues = values(phaseFunctionInputs);
            layerRegions = input.LayerRegions;
            for i=1:length(input.LayerRegions)
                zRange = layerRegions(i).ZRange;
                regionOP = layerRegions(i).RegionOP;
                regionKey = layerRegions(i).PhaseFunctionKey;
                % Note LayerTissueRegion knows only about the keys not the the values
                regionsNET(i) = Vts.MonteCarlo.Tissues.LayerTissueRegion(...
                    Vts.Common.DoubleRange( ...
                        zRange(1), ...
                        zRange(2) ...
                        ), ...
                    Vts.OpticalProperties( ...
                        regionOP(1), ...
                        regionOP(2), ...
                        regionOP(3), ...
                        regionOP(4) ...
                        ),...
                    regionKey...
                 );
            end
            
            inputNET = Vts.MonteCarlo.Tissues.MultiLayerTissueInput(regionsNET);
            temp = NET.createGeneric('System.Collections.Generic.Dictionary', {'System.String' ,'Vts.MonteCarlo.IPhaseFunctionInput'});
            for i=1:length(input.LayerRegions)
                if (phaseFunctionValues{i}.PhaseFunctionType == 'HenyeyGreenstein')
                    phaseFunctionValue = Vts.MonteCarlo.HenyeyGreensteinPhaseFunctionInput();
                elseif (phaseFunctionValues{i}.PhaseFunctionType == 'LookupTable')
                    phaseFunctionValue = Vts.MonteCarlo.LookupTablePhaseFunctionInput();
                elseif (phaseFunctionValues{i}.PhaseFunctionType == 'Bidirectional')
                    phaseFunctionValue = Vts.MonteCarlo.LookupTablePhaseFunctionInput();
                end
                temp.Add(phaseFunctionKeys{i},phaseFunctionValue);
             end
            inputNET.RegionPhaseFunctionInputs = temp;

        end
    end
end