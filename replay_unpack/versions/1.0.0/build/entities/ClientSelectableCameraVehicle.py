#      DO NOT CHANGE THIS FILE     #
# FILE WAS GENERATED AUTOMATICALLY #

from def_generator.events import EventHook
from operator import itemgetter

from def_generator.decorators import unpack_func_args, unpack_variables




class ClientSelectableCameraVehicle(object):
    
    def __init__(self):
        self.id = None
        self.position = None


        self._modelName = None

        self._vehicleGunPitch = None

        self._vehicleTurretYaw = None


        # MRO fix

        self._properties = getattr(self, '_properties', [])
        self._properties.extend([
            (10000000000, 'modelName'),
            (32, 'vehicleGunPitch'),
            (32, 'vehicleTurretYaw'),
            
        ])
        # sort properties by size
        self._properties.sort(key=itemgetter(0))

        self._methods = getattr(self, '_methods', [])
        self._methods.extend([
            
        ])
        # sort methods by size
        self._methods.sort(key=itemgetter(0))
        return

    @property
    def attributesMap(self):
        d = {}
        for i, (_, name) in enumerate(self._properties):
            d[i] = name
        return d

    @property
    def methodsMap(self):
        d = {}
        for i, (_, name) in enumerate(self._methods):
            d[i] = name
        return d

    ####################################
    #      METHODS
    ####################################



    ####################################
    #       PROPERTIES
    ####################################


    # property size: 10000000000
    @property
    def modelName(self):
        return self._modelName

    @modelName.setter
    def modelName(self, value):
        self._modelName, = unpack_variables(value, ['STRING'])

    # property size: 32
    @property
    def vehicleGunPitch(self):
        return self._vehicleGunPitch

    @vehicleGunPitch.setter
    def vehicleGunPitch(self, value):
        self._vehicleGunPitch, = unpack_variables(value, ['FLOAT32'])

    # property size: 32
    @property
    def vehicleTurretYaw(self):
        return self._vehicleTurretYaw

    @vehicleTurretYaw.setter
    def vehicleTurretYaw(self, value):
        self._vehicleTurretYaw, = unpack_variables(value, ['FLOAT32'])


    def __repr__(self):
        d = {}
        for _, p in self._properties:
            d[p] = getattr(self, p)
        return "<{}> {}".format(self.__class__.__name__, d)