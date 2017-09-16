using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ITST.Models;

namespace ITST.ViewModels
{
    public class DashboardViewModels
    {
        public int SumUserCenter { get; set; }
        public int SumMachineCenter { get; set; }
        public int SumDeviceCenter { get; set; }

        public int SumUserTBS { get; set; }
        public int SumMachineTBS { get; set; }
        public int SumDeviceTBS { get; set; }

        public int SumUserPCLT { get; set; }
        public int SumMachinePCLT { get; set; }
        public int SumDevicePCLT { get; set; }

        public int TotalDevice { get; set; }
        public int TotalMachine { get; set; }
        public int TotalUser { get; set; }
        public int TotalInStock { get; set; }
        public int TotalInUse { get; set; }
        public int TotalInRepair { get; set; }
        public int TotalSentRepair { get; set; }
        public int TotalSpare { get; set; }
        public int TotalSale { get; set; }
        public int TotalSentSale { get; set; }
        public int TotalWaitSentSale { get; set; }
        public int TotalWaitSentRepair { get; set; }

        public int TotalMachineHR { get; set; }
        public int TotalMachineIT { get; set; }
        public int TotalMachineTechnology { get; set; }
        public int TotalMachineQA { get; set; }
        public int TotalMachinePlantControl { get; set; }
        public int TotalMachineProduction { get; set; }
        public int TotalMachineSE { get; set; }
        public int TotalMachinePC { get; set; }
        public int TotalMachineBOI { get; set; }
        public int TotalMachineFA { get; set; }
        public int TotalMachinePurchasing { get; set; }
        public int TotalMachineTPMTPS { get; set; }


        public int TotalUserBOI { get; set; }
        public int TotalUserFA { get; set; }
        public int TotalUserHR { get; set; }
        public int TotalUserIT { get; set; }
        public int TotalUserTechnology { get; set; }
        public int TotalUserQA { get; set; }
        public int TotalUserPlantControl { get; set; }
        public int TotalUserProduction { get; set; }
        public int TotalUserSE { get; set; }
        public int TotalUserPC { get; set; }
        public int TotalUserPurchasing { get; set; }
        public int TotalUserTPMTPS { get; set; }


        public int TotalDeviceBuildingTBS { get; set; }
        public int TotalDeviceCuringTBS { get; set; }
        public int TotalDeviceTechnologyTBS { get; set; }
        public int TotalDeviceFinishingTBS { get; set; }
        public int TotalDeviceElectricalTBS { get; set; }
        public int TotalDeviceMixingTBS { get; set; }
        public int TotalDeviceRawMaterialTBS { get; set; }
        public int TotalDeviceDistributeTBS { get; set; }
        public int TotalDeviceMaintenanceTBS { get; set; }
        public int TotalDeviceMatPrepareTBS { get; set; }
        public int TotalDeviceQATBS { get; set; }
        public int TotalDevicePlantControlTBS { get; set; }
        public int TotalDeviceProductionTBS { get; set; }
        public int TotalDeviceHRTBS { get; set; }
        public int TotalDeviceITTBS { get; set; }

        public int TotalDeviceBuildingCenter { get; set; }
        public int TotalDeviceCuringCenter { get; set; }
        public int TotalDeviceTechnologyCenter { get; set; }
        public int TotalDeviceTPMTPSCenter { get; set; }
        public int TotalDeviceFinishingCenter { get; set; }
        public int TotalDeviceElectricalCenter { get; set; }
        public int TotalDeviceMixingCenter { get; set; }
        public int TotalDeviceRawMaterialCenter { get; set; }
        public int TotalDeviceDistributeCenter { get; set; }
        public int TotalDeviceMaintenanceCenter { get; set; }
        public int TotalDeviceMatPrepareCenter { get; set; }
        public int TotalDeviceQACenter { get; set; }
        public int TotalDevicePlantControlCenter { get; set; }
        public int TotalDeviceProductionCenter { get; set; }
        public int TotalDevicePCCenter { get; set; }
        public int TotalDeviceSECenter { get; set; }
        public int TotalDeviceHRCenter { get; set; }
        public int TotalDeviceITCenter { get; set; }
        public int TotalDeviceBOICenter { get; set; }
        public int TotalDeviceFACenter { get; set; }
        public int TotalDevicePurchasingCenter { get; set; }


        public int TotalDeviceBuildingPCLT { get; set; }
        public int TotalDeviceCuringPCLT { get; set; }
        public int TotalDeviceTechnologyPCLT { get; set; }
        public int TotalDeviceFinishingPCLT { get; set; }
        public int TotalDeviceElectricalPCLT { get; set; }
        public int TotalDeviceMixingPCLT { get; set; }
        public int TotalDeviceRawMaterialPCLT { get; set; }
        public int TotalDeviceDistributePCLT { get; set; }
        public int TotalDeviceMaintenancePCLT { get; set; }
        public int TotalDeviceMatPreparePCLT { get; set; }
        public int TotalDeviceQAPCLT { get; set; }
        public int TotalDevicePlantControlPCLT { get; set; }
        public int TotalDeviceProductionPCLT { get; set; }
        public int TotalDeviceHRPCLT { get; set; }
        public int TotalDeviceITPCLT { get; set; }

        public int TotalMachineBuildingTBS { get; set; }
        public int TotalMachineCuringTBS { get; set; }
        public int TotalMachineTechnologyTBS { get; set; }
        public int TotalMachineFinishingTBS { get; set; }
        public int TotalMachineElectricalTBS { get; set; }
        public int TotalMachineMixingTBS { get; set; }
        public int TotalMachineRawMaterialTBS { get; set; }
        public int TotalMachineDistributeTBS { get; set; }
        public int TotalMachineMaintenanceTBS { get; set; }
        public int TotalMachineMatPrepareTBS { get; set; }
        public int TotalMachineQATBS { get; set; }
        public int TotalMachinePlantControlTBS { get; set; }
        public int TotalMachineProductionTBS { get; set; }
        public int TotalMachineHRTBS { get; set; }
        public int TotalMachineITTBS { get; set; }

        public int TotalUserBuildingTBS { get; set; }
        public int TotalUserCuringTBS { get; set; }
        public int TotalUserTechnologyTBS { get; set; }
        public int TotalUserFinishingTBS { get; set; }
        public int TotalUserElectricalTBS { get; set; }
        public int TotalUserMixingTBS { get; set; }
        public int TotalUserRawMaterialTBS { get; set; }
        public int TotalUserDistributeTBS { get; set; }
        public int TotalUserMaintenanceTBS { get; set; }
        public int TotalUserMatPrepareTBS { get; set; }
        public int TotalUserQATBS { get; set; }
        public int TotalUserPlantControlTBS { get; set; }
        public int TotalUserProductionTBS { get; set; }
        public int TotalUserHRTBS { get; set; }
        public int TotalUserITTBS { get; set; }

        public int TotalUserBuildingPCLT { get; set; }
        public int TotalUserCuringPCLT { get; set; }
        public int TotalUserTechnologyPCLT { get; set; }
        public int TotalUserFinishingPCLT { get; set; }
        public int TotalUserElectricalPCLT { get; set; }
        public int TotalUserMixingPCLT { get; set; }
        public int TotalUserRawMaterialPCLT { get; set; }
        public int TotalUserDistributePCLT { get; set; }
        public int TotalUserMaintenancePCLT { get; set; }
        public int TotalUserMatPreparePCLT { get; set; }
        public int TotalUserQAPCLT { get; set; }
        public int TotalUserPlantControlPCLT { get; set; }
        public int TotalUserProductionPCLT { get; set; }
        public int TotalUserHRPCLT { get; set; }
        public int TotalUserITPCLT { get; set; }

        public int TotalMachineBuildingPCLT { get; set; }
        public int TotalMachineCuringPCLT { get; set; }
        public int TotalMachineTechnologyPCLT { get; set; }
        public int TotalMachineFinishingPCLT { get; set; }
        public int TotalMachineElectricalPCLT { get; set; }
        public int TotalMachineMixingPCLT { get; set; }
        public int TotalMachineRawMaterialPCLT { get; set; }
        public int TotalMachineDistributePCLT { get; set; }
        public int TotalMachineMaintenancePCLT { get; set; }
        public int TotalMachineMatPreparePCLT { get; set; }
        public int TotalMachineQAPCLT { get; set; }
        public int TotalMachinePlantControlPCLT { get; set; }
        public int TotalMachineProductionPCLT { get; set; }
        public int TotalMachineHRPCLT { get; set; }
        public int TotalMachineITPCLT { get; set; }
    }

    public class ViewModelSerialNumberGenerate
    {
        public int GenerateID { get; set; }
        public string SerialNumber { get; set; }
        public string DeviceType { get; set; }
        public string CreateBy { get; set; }
        public string UpdateBy { get; set; }
        public Nullable<System.DateTime> DateCreate { get; set; }
        public Nullable<System.DateTime> DateUpdate { get; set; }
        public string IsUse { get; set; }
    }
}