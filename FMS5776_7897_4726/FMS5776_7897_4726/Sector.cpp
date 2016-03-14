#include "Sector.h"

int& Sector::operator[](int index){ return RawData[index]; }