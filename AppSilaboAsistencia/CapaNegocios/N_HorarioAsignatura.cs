﻿using CapaDatos;
using CapaEntidades;
using System.Data;

namespace CapaNegocios
{
    public class N_HorarioAsignatura
    {
        readonly D_HorarioAsignatura ObjHorarioAsignatura = new D_HorarioAsignatura();

        public static DataTable BuscarHorarioAsignatura(string CodSemestre, string CodDepartamentoA, string CodEscuelaP, string Texto, string Grupo)
        {
            return new D_HorarioAsignatura().BuscarHorarioAsignatura(CodSemestre, CodDepartamentoA, CodEscuelaP, Texto, Grupo);
        }

        public static DataTable HorarioSemanalDocente(string CodSemestre, string Texto)
        {
            return new D_HorarioAsignatura().HorarioSemanalDocente(CodSemestre, Texto);
        }

        public void InsertarHorarioAsignatura(E_HorarioAsignatura HorarioAsignatura)
        {
            ObjHorarioAsignatura.InsertarHorarioAsignatura(HorarioAsignatura);
        }

        public void ActualizarHorarioAsignatura(E_HorarioAsignatura HorarioAsignatura)
        {
            ObjHorarioAsignatura.ActualizarHorarioAsignatura(HorarioAsignatura);
        }

        public void EliminarHorarioAsignatura(E_HorarioAsignatura HorarioAsignatura)
        {
            ObjHorarioAsignatura.EliminarHorarioAsignatura(HorarioAsignatura);
        }
    }
}
